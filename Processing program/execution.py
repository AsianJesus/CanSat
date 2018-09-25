import time, threading, random, queue
import basicio, processing,distributor,dbproxy,fileio 
from cuiinterface import Processes,Queues,CUI
from session import Session
from flag import Flag
import exceptionhandler
import config
import os

configuration:config.Configuration = None
flags = {}
queues = {}
defInfo = {"sesID":"0","sesName":"None","host":"localhost","user":"root","password":"1123581321ElViNBV","schema":"pourtest","port":"COM6","baudrate":"9600"}
defCommands = {"ins_data" : "INSERT INTO data(flyID,packetID,flyTime,temp,press,height,humidity,speed,voltage,gpsX,gpsY,gpsZ,co2,no2,nh3,utcTime) VALUES(%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s);",
    "ins_id" : "INSERT INTO flights(id,name) VALUES(%s,%s) ON DUPLICATE UPDATE;"}
commands = {
        "release"           :0b00000010,
        "reset_mechanism"   :0b00000100,
        "reset"             :0b00001000,
        "beep_start"        :0b00010000,
        "beep_stop"         :0b00100000,
        "take_photo"        :0b01000000,
        "save_pressure"     :0b10000000
    }
curSession:Session = None   
currInfo = {}
dbCommands = {}
confName = "config.xml"
threads = {}
excHandler : exceptionhandler.ExceptionHandler = None
excFile : fileio.FileIO= None

def __clearScreen():
    os.system("clear")
    os.system("cls")

def interactiveConfigChange():
    global currInfo,dbCommands
    print("What would you change?")
    toContinue = True
    while(toContinue):
        toContinue = __selectConfig()
    configuration.saveInfo(currInfo,dbCommands)

def __selectConfig()->bool:
    global currInfo,dbCommands
    print(("***"*20).center(100,"-"))
    codes = {str(code):info for code,info in enumerate(currInfo)}
    codes.update({str(code+len(currInfo)):info for code,info in enumerate(dbCommands)})
    for (code,info) in codes.items():
        print("({code:1}){info:6}\t".format(info=info,code=code),end=' ')
    print("\n(s)how config","(r)eset configuration","(e)xit configuration","(q)uit program",sep='\t')
    select = input("")
    select = select.lower()
    if select == "s":
        __showConfig()
        return True;
    elif select == "r":
        restoreConfig()
    elif select == "e":
        return False
    elif select == "q":
        exit(10)
    elif select in [str(code) for code,c in enumerate(currInfo)]:
        value = input("Input value for {}: ".format(codes[select]))
        currInfo[codes[select]] = value
    elif select in [str(code+len(currInfo)) for code,c in enumerate(dbCommands)]:
        value = input("Input value for {}: ".format(codes[select]))
        dbCommands[codes[select]] = value
    __clearScreen()
    return True

def loadSessionInfo():
    global currInfo
    if currInfo["sesID"] == "0" or not currInfo["sesID"].isnumeric():
        print("There is no saved session")
    else:
        savedID,savedName = currInfo["sesID"],currInfo["sesName"]
        print("We have saved session - ID {} Name {}".format(str(savedID),str(savedName)),"Would you like to change session?",sep='\n')
        choice = ""
        while choice != 'y' and choice != 'n':
            choice = input("(Y)es\t(N)o\n")
            choice = choice.lower()
        if choice == 'n':
            return
    sesID = ""
    while not sesID.isnumeric():
        sesID = input("Input session id: ")
    sesName = input("Input session name: ")
    currInfo["sesID"] = sesID
    currInfo["sesName"] = sesName
        
def startSession():
    global curSession
    curSession = Session(int(currInfo["sesID"]),currInfo["sesName"])


def createExceptionhandler():
    global excHandler,excFile,queues
    errorQueueFileIO = queue.Queue(20)
    excFile = fileio.FileIO("error_log.txt",errorQueueFileIO,Flag(True))
    queues["error"] = queue.Queue(40)
    excHandler = exceptionhandler.ExceptionHandler(queues["error"],errorQueueFileIO)
    basicio.BasicIO.setErrorLog(queues["error"])
    processing.DataHandler.setErrorLog(queues["error"])
    distributor.Distributor.setErrorLog(queues["error"])
    dbproxy.DataBaseWorker.setErrorLog(queues["error"])
    fileio.FileIO.setErrorLog(queues["error"])
    excHandler.start()
    excFile.start()


def readConfig(filename)->(dict,dict):
    global configuration
    configuration = config.Configuration(filename)
    return (configuration.getAllInfo(),configuration.getCommands(),)

def __showConfig():
    print("~~~"*20)
    print("Available information: ")
    print("\t\tInfo: ")
    for tag,info in currInfo.items():
        print("{:10}\t{}".format(tag,info))
    print("\t\tCommands:")
    for comName,comBody in dbCommands.items():
        print("{:10}\t{:100}".format(comName,comBody))
    print("~~~"*20)
    

def restoreConfig():
    global currInfo,dbCommands
    currInfo,dbCommands = defInfo,defCommands
    configuration.saveInfo(defInfo,defCommands)
    

def changeConfig(tag,value):
    if tag == "baudrate":
        try:
            int(value)
        except:
            return
    configuration.set(tag,value)
    if tag in defInfo:
        global currInfo
        currInfo[tag] = value
    if tag in defCommands:
        global dbCommands
        dbCommands[tag] = value

def createQueues():
    global queues
    for q,qc in Queues.getAll():
        queues[q] = queue.Queue(100)

def createFlags():
    global flags
    for process,processCode in Processes.getAll():
        flags[process] = Flag(False)

def prepareThreads():
    global threads
    dbproxy.DBCommand.comList = dbCommands
    threads[Processes.BasicIO] = basicio.BasicIO(
                    currInfo["port"],int(currInfo["baudrate"]),
                    queues[Queues.XBeeInput],queues[Queues.XBeeOutput],flags[Processes.BasicIO])
    threads[Processes.Processing] = processing.DataHandler(queues[Queues.XBeeOutput],queues[Queues.ProcessingOutput],flags[Processes.Processing])
    threads[Processes.Distribution] = distributor.Distributor(queues[Queues.ProcessingOutput],
                                                              queues[Queues.DatabaseInput],queues[Queues.FirstFileInput],queues[Queues.SecondFileInput],
                                                              flags[Processes.Distribution],curSession)
    threads[Processes.DatabaseIO] = dbproxy.DataBaseWorker(currInfo["host"],currInfo["user"],currInfo["password"],currInfo["schema"],
                                                           queues[Queues.DatabaseInput],queue.Queue(),flags[Processes.DatabaseIO])
    threads[Processes.FileIO] = (fileio.FileIO("out(1).csv",queues[Queues.FirstFileInput],flags[Processes.FileIO]),
                                 fileio.FileIO("out(2).csv",queues[Queues.SecondFileInput],flags[Processes.FileIO]))

def startThreads():
    global flags,threads
    for threadDesk,thread in threads.items():
        if hasattr(thread,"__iter__"):
            for t in thread:
                t.start()
        else:
            thread.start()

def terminateThreads():
    for threadDesk,thread in threads.items():
        if hasattr(thread,"__iter__"):
            for t in thread:
                t.terminate()
        else:
            thread.terminate()

def terminateExceptionHandler():
    excHandler.terminate()
    excFile.terminate()

def startCUI():
    global curSession,currInfo,flags,queues,commands
    interface = CUI(curSession,flags,queues,commands)
    interface.start()
    interface.join()

def __main__():
    global currInfo,dbCommands
    (readDBinfo,readComms) = readConfig(confName)
    if [k for k in defInfo if k not in readDBinfo] or [k for k in defCommands if k not in readComms]:
        restoreConfig()
        print("+-"*30,"Invalid configuration has been restored","-+"*30,sep='\n')
    else:
        currInfo, dbCommands = readDBinfo,readComms
        print("+-"*30,"Saved configuration successfully loaded","-+"*30,sep='\n')
    
    loadSessionInfo()
    interactiveConfigChange()

    startSession()

    createQueues()
    createFlags()
    createExceptionhandler()

    prepareThreads()

    startThreads()
    
    startCUI()

    currInfo["sesID"] = str(curSession.getID())
    currInfo["sesName"] = str(curSession.getName())
    configuration.saveInfo(currInfo,dbCommands)

    terminateThreads()
    terminateExceptionHandler()

__main__()