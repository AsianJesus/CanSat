import time, threading, random, queue
import basicio, processing,distributor,dbproxy,fileio 
from cuiinterface import Processes,Queues,CUI
import session,flag
import exceptionhandler

class QueueShower(threading.Thread):
    def __init__(self,q):
        self.q = q
        self.exitFlag = False
        super().__init__()
    def run(self):
        while True:
            if self.exitFlag:
                break
            while not self.q.empty():
                element = self.q.get()
                if element:
                    print(element.getInfo())
            time.sleep(1/2)
    def terminate(self):
        self.exitFlag = True

host = "localhost"
uid = "root"
passw = "1123581321ElViNBV"
schema = "pourtest"

dbproxy.DBCommand.comList = {
    "ins_data" : "INSERT INTO data VALUES(%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s);",
    "ins_id" : "INSERT INTO flights(id,name) VALUES(%s,%s);"
    }

exceptionOutput = queue.Queue(20)
fileOutput = queue.Queue(20)

flags = {p[0] : flag.Flag(False) for p in Processes.getAll() }
queues = {q[0] : queue.Queue(50) for q in Queues.getAll()}

#sID = ""
#while not sID.isnumeric():
#    sID = input("Input session id: ")
sID = 8
sName = ""
#sName = input("Input session name: ")

ses = session.Session(sID,sName)

expOut = fileio.FileIO("log.txt",fileOutput,flag.Flag())
expHandler = exceptionhandler.ExceptionHandler(exceptionOutput,fileOutput)

processing.DataHandler.setErrorLog(exceptionOutput)
distributor.Distributor.setErrorLog(exceptionOutput)
dbproxy.DataBaseWorker.setErrorLog(exceptionOutput)
fileio.FileIO.setErrorLog(exceptionOutput)

xbeeWorker = basicio.BasicIO("COM6",9600,queues[Queues.XBeeInput],queues[Queues.XBeeOutput],flags[Processes.BasicIO])
process = processing.DataHandler(queues[Queues.XBeeOutput],queues[Queues.ProcessingOutput],flags[Processes.Processing])
distrib = distributor.Distributor(queues[Queues.ProcessingOutput],queues[Queues.DatabaseInput],queues[Queues.FirstFileInput],queues[Queues.SecondFileInput],flags[Processes.Distribution],ses)
dbWorker = dbproxy.DataBaseWorker(host,uid,passw,schema,queues[Queues.DatabaseInput],queue.Queue(100),flags[Processes.DatabaseIO])
fWorkerf = fileio.FileIO("output(1).csv",queues[Queues.FirstFileInput],flags[Processes.FileIO])
fWorkers = fileio.FileIO("output(2).csv",queues[Queues.SecondFileInput],flags[Processes.FileIO])

interface = CUI(ses,flags,queues)

process.start()
distrib.start()
dbWorker.start()
fWorkerf.start()
fWorkers.start()
expOut.start()
expHandler.start()

interface.start()
interface.join()

xbeeWorker.terminate()
process.terminate()
distrib.terminate()
dbWorker.terminate()
fWorkerf.terminate()
fWorkers.terminate()
expOut.terminate()
expHandler.terminate()

exit(0)
