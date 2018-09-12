from threading import Thread
from flag import Flag
from session import Session
from enum import Enum
import basicio
import queue
import os

class Processes(Enum):
    BasicIO = 'xbeeio'
    Processing = 'processing'
    Distribution = 'distrib'
    DatabaseIO = 'dbworker'
    FileIO = 'fileio'
    def getAll()->tuple:
        return ((Processes.BasicIO,'xbeeio'),(Processes.Processing,'processing'),(Processes.Distribution,'distrib'),(Processes.DatabaseIO,'dbworker'),(Processes.FileIO,'fileio'))
    def doesExists(element:str)->bool:
        return element in [x[1] for x in Processes.getAll()]
    def getByValue(elem: str):
        if not Processes.doesExists(elem):
            return None
        for (key,val) in Processes.getAll():
            if val == elem:
                return key
    def getByName(elem):
        if elem not in [x[0] for x in Processes.getAll()]:
            return None;
        for (key,val) in Processes.getAll():
            if key == elem:
                return val

class Queues(Enum):
    XBeeInput = 'xbeein'
    ProcessingOutput = 'procout'
    XBeeOutput = 'xbeeout'
    DatabaseInput = 'dbin'
    FirstFileInput = 'ffilein'
    SecondFileInput = 'sfilein'
    def getAll()->tuple:
        return ((Queues.XBeeInput,'xbeein'),(Queues.XBeeOutput,"xbeeout"),(Queues.ProcessingOutput,'procout'),(Queues.DatabaseInput,'dbin'),(Queues.FirstFileInput,'ffilein'),(Queues.SecondFileInput,'sfilein'))
    def doesExist(elem:str)->bool:
        return elem in [x[1] for x in Queues.getAll()]
    def getByValue(elem: str):
        if not Queues.doesExist(elem):
            return None
        for (key,val) in Queues.getAll():
            if val == elem:
                return key
    def getByName(elem):
        if elem not in [x[0] for x in Queues.getAll()]:
            return None;
        for (key,val) in Queues.getAll():
            if key == elem:
                return val

class Command():
    def __init__(self,help,execution,verification = lambda args: True):
        self.ver = verification
        self.help = help
        self.exec = execution
    def showHelp(self):
        self.help()
    def verify(self,args) -> bool:
        return self.ver(args)
    def execute(self,s,args)->None:
        self.exec(s,args)

class ProcessesController:
    def __init__(self,flags:{}):
        self.curFlags = {Processes.getByName(name):flag for name,flag in flags.items()}
        self.savedFlags = {}
    def stopAll(self):
        for process in self.curFlags:
            self.stop(process)
    def stop(self,process):
        self.curFlags[process].setState(False)
    def startAll(self):
        for process in self.curFlags:
            self.start(process)
    def start(self,process):
        self.curFlags[process].setState(True)
    def pause(self,process):
        self.savedFlags[process] = self.curFlags[process]
        self.curFlags[process].setState(False)
    def pauseAll(self):
        for process in self.curFlags:
            self.pause(process)
    def resume(self,process):
        if process in self.savedFlags:
            self.curFlags[self.savedFlags[process].getState()]
    def resumeAll(self):
        for process in self.curFlags:
            self.resume(process)

class QueuesList:
    def __init__(self,queues):
        self.queues = queues
    def showQueue(self,queue):
        curQueue = self.queues[queue] if queue in self.queues else self.queues[Queues.getByValue(queue)]
        if curQueue is not None:
            print("{}:".format(queue))
            for item in curQueue.queue:
                print("","{}".format(item),sep='\t',end='\n')
    def showQueuesList(self):
        print("Queues list: ",'',sep='\n')
        for queue in self.queues:
            print("{}\t{}".format(queue,Queues.getByName(queue)))
        print("")
    def doesQueueExist(self,queue)->bool:
        return queue in self.queues
    def putIntoQueue(self,q,value):
        qq = queue.Queue(10)
        if q in [x[0] for x in Queues.getAll()]:
            qq = self.queues[q]
        elif q in [x[1] for x in Queues.getAll()]:
            qq = self.queues[Queues.getByValue(q)]
        qq.put(value)

class CUI(Thread):
    #Command : comment
    comList = {
       "stop" : Command(
                        verification=lambda args: args and Processes.doesExists(args[0]),
                        help=lambda: print("Stop process. Using: stop xbeeio"),
                        execution=lambda self,args: self.procControl.stop(args[0])),
       "stop_all" : Command(
                        help=lambda: print("Stop all processes. Using: stop_all"),
                        execution=lambda self,args: self.procControl.stopAll()),
       "start" : Command(
                        verification=lambda args: args and Processes.doesExists(args[0]),
                        help=lambda: print("Start process. Using: start xbeeio"),
                        execution=lambda self,args: self.procControl.start(args[0])),
       "start_all" : Command(
                        help=lambda: print("Start all processes: Using: start_all"),
                        execution=lambda self,args: self.procControl.startAll()),       
       "pause_all": Command(
                        help=lambda: print("Pause all processes: Using: pause_all"),
                        execution=lambda self,args: self.procControl.pauseAll()),
       "pause" : Command(
                        verification=lambda args: args and Processes.doesExists(args[0]),
                        help=lambda: print("Pause process. Using: pause fileio"),
                        execution=lambda self,args: self.procControl.pause(args[0])),
       "resume" : Command(verification=lambda args: args and Processes.doesExists(args[0]),
                        help=lambda: print("Resume paused process. Using: resume fileio"),
                        execution=lambda self,args: self.procControl.resume(args[0])),
       "resume_all" : Command(
                        help=lambda: print("Resume all paused process. Using: resume_all"),
                        execution=lambda self,args: self.procControl.resumeAll()),
       "show_queues" : Command(
                        help=lambda: print("Shows all queues. Using: show_queues"),
                        execution=lambda self,args: self.__showQueues()),
       "show_queue" : Command(
                        verification=lambda args: args and Queues.doesExist(args[0]),
                        help=lambda: print("Shows all queue. Using: show_queue queue"),
                        execution=lambda self,args: self.__showQueue(args[0])),
       "show_processes" : Command(
                        help=lambda: print("Prints all available processes. Using: show_processes"),
                        execution=lambda self,args: CUI.__printProcesses()
                                  ),
       "show_states":Command(
                        help=lambda: print("Show processes states. Using: show_states"),
                        execution=lambda self,args: self.__showStates()),
       "session_show" : Command(
                        help=lambda: print("Stop current session. Using: session_stop"),
                        execution=lambda self,args: self.__showSession()),
       "session_stop" : Command(
                        help=lambda: print("Start current session. Using: session_start"),
                        execution=lambda self,args: self.__stopSession()),
       "session_start" : Command(
                        help=lambda: print("Stop all processes. Using: stop_all"),
                        execution=lambda self,args: self.__startSession()),
       "session_change" : Command(
                        verification=lambda args: args and int(args[0]),
                        help=lambda: print("Change session. Using: session_change 5"),
                        execution=lambda self,args: self.__changeSession(args)),
       "help" : Command(
                        verification=lambda args: args[0] in CUI.comList if args else True,
                        help=lambda: print("Show info about command. Using: help []"),
                        execution=lambda self,args: CUI.__showCommandInfo(args[0]) if args else CUI.__showCommands() ),
       "exit" : Command(
                        help = lambda: print("Closes program"),
                        execution = lambda self,args: self.__exit()
           ),
       "show_commands" : Command(
                        help=lambda: print("Showes available commands"),
                        execution=lambda self,args: self.__showMessages()
           ),
       "send_command" : Command(
                        help=lambda: print("Sends command to satellite"),
                        verification=lambda args: args and ("".join(args) in CUI.messages or "".join(args) in [item for key,item in CUI.messages.items()]),
                        execution=lambda self,args: self.__sendMessage("".join(args)) 
           ),
       "clear" : Command(
                        help=lambda: print("Clear screen"),
                        execution=lambda self,args: CUI.__clearScreen()
           ),
       "reset" : Command(
                        verification=lambda args: args and int(args[0]),
                        help=lambda: print("Send \"reset\" command and change session. Using: reset 5"),
                        execution=lambda self,args: self.__changeSession(args))
        }
    messages : dict
    def __init__(self,ses:Session,flags:{str:Flag},queues:dict,msg:dict): 
        self.session = ses
        self.flags = flags
        self.procControl = ProcessesController(self.flags)
        self.exitFlag = False
        self.queues = QueuesList(queues)
        CUI.messages = msg
        super().__init__()
    def start(self):
        super().start()
    def end(self):
        self.exitFlag = True
    def run(self):
        while True:
            if self.exitFlag:
                break
            rawCom = CUI.waitCommand()
            if not rawCom:
                continue
            args = rawCom[1:]
            com = rawCom[0]
            self.__handleCommand(com,args)
    def waitCommand()->tuple():
        com = input(">>>")
        return [x for x in com.split(' ') if x != '']
    def __handleCommand(self,comm:str,args:tuple):
        if comm not in CUI.comList:
            CUI.__showHelp()
            return False
        command = CUI.comList[comm]
        if command.verify(args):
            command.execute(self,args)
        else:
            command.showHelp()
    def __showSession(self):
        print("ID\tName\n{0}\t{1}".format(self.session.getID(),self.session.getName()))
    def __stopSession(self):
        self.session.stop()
    def __startSession(self):
        self.session.start()
    def __changeSession(self,args):
        id = int(args[0])
        self.session.setID(id)
        if len(args) >= 2:
            self.session.setName(args[1])
    def __showCommands():
        for command,body in CUI.comList.items():
            print("{:20} | ".format(command),end='')
            body.showHelp()
    def __printProcesses():
        print("Invalid process")
        print("List of available processes:")
        for (proc,code) in Processes.getAll():
            print("{0:60}         \t{1}".format(proc,code))
    def __showQueues(self):
        self.queues.showQueuesList()
    def __showQueue(self,q):
        print("Queue - {:50}:".format(Queues.getByValue(q)))
        self.queues.showQueue(q)
    def __showHelp():
        print("Invalid command\nTo view all commands type 'help' .")
    def __showCommandInfo(command:str):
        print("{0:50}\t{1:30}".format(command,CUI.comList[command]))
    def __showStates(self):
        for name,state in self.flags.items():
            print("Process - {0:50}\tActive? - {1:30}".format(name,state.getState()))
    def __exit(self):
        print("Goodbye")
        self.exitFlag = True
    def __showMessages(self):
        print("-"*5,"+"*10,"-"*5,"\nAvailable commands:",sep='')
        for comName,comDesc in CUI.messages.items():
            print("{:15} - {:30}".format(comName,comDesc))
    def __sendMessage(self,args):
        self.queues.putIntoQueue(Queues.XBeeInput,basicio.SatCommand(CUI.messages[args],lambda: print("Command sent")))
        print("We successfully sended {} command".format(args))
    def __clearScreen():
        os.system("clear")
        os.system("cls")
    def __reset(self,args):
        self.queues.putIntoQueue(Queues.XBeeInput,basicio.SatCommand(CUI.messages["reset"],self.__changeSession(int(args[0]))))