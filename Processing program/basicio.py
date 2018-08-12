from serial import Serial
from xbee.thread import XBee
from threading import Thread
from queue import Queue
from cuiinterface import Flag
from enum import Enum
from time import sleep

class XBeeInterface:
    def __init__(self,port:str,baudRate:int):
        pass
    def open(self)->None:
        pass
    def close(self)->None:
        pass
    def isOpen(self)->bool:
        pass
    def send(self,msg:str)->None:
        pass
    def read(self)->str:
        pass

class BasicIOCommands(Enum):
    send = 1
    read = 0
    

class BasicIO(Thread):
    errorLog:Queue
    def __init__(self,port:str,baudRate:int,comInput: Queue,output:Queue,opFlag:Flag, logFileName:str = "basicIO_log.txt"):
        self.XBee = XBeeInterface(port,baudRate)
        self.output = output
        self.opFlag = opFlag
        self.comInput = comInput
        self.exitFlag = False
        self.log = open(logFileName,"a");
        super().__init__()
    def start(self):
        self.opFlag.setState(True)
        super().start()
    def run(self):
        while True:
            if self.exitFlag:
                break
            sleep(1/4)
            if not self.opFlag.getState():
                continue
            self.__operate()            
    def stop(self):
        self.opFlag.setState(False)
    def __operate(self)->None:
        if not self.comInput.empty():
             comm = self.comInput.get()
             self.log.write("Got command - {}\n".format(comm))
             self.log.flush()
             if isinstance(comm,str):
                self.comHandler(comm)
        else:
            result = self.read()
            if result is not None:
                self.output.put(result)
                self.log.write("Got message - {}\n".format(result))
                self.log.flush()

    def read(self)->str:
        try:
            result = self.XBee.read()
            return result
        except Exception as e:
            BasicIO.errorLog.put(e)
        return ""
    def write(self,msg:str):
        try:
            self.XBee.send(msg)
        except Exception as e:
            BasicIO.errorLog.put(e)
    def comHandler(self,com):
        try:
                write(com)
        except Exception as e:
                BasicIO.errorLog.put(e)
    def setErrorLog(log: Queue):
        BasicIO.errorLog = log
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
        self.log.close();
