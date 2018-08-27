from serial import Serial
from threading import Thread
from queue import Queue
from cuiinterface import Flag
from enum import Enum
from time import sleep
from hashlib import md5

class XBeeInterface:
    def __init__(self,port:str,baudRate:int):
        self.sPort = Serial(port=port,baudrate=baudRate);
        self.portName,self.baudRate = port ,baudRate
    def open(self)->None:
        self.sPort.open();
        pass
    def close(self)->None:
        self.sPort.close();
    def isOpen(self)->bool:
        return self.sPort.isOpen();
    def send(self,msg:str)->None:
        if not self.isOpen:
            self.open()
        txt = msg + "|" + CalculateHash(msg) + "\0"
        self.sPort.write(txt.encode('ASCII'))
    def read(self):
        if not self.isOpen:
            self.open()
        msg = self.sPort.read_all().decode('ASCII');
        return msg.split('\n')


class BasicIOCommands(Enum):
    send = 1
    read = 0
    
def CalculateHash(txt):
    return md5(txt.encode('ASCII')).hexdigest() 

class BasicIO(Thread):
    errorLog:Queue
    def __init__(self,port:str,baudRate:int,comInput: Queue,output:Queue,opFlag:Flag, logFileName:str = "basicIO_log.txt"):
        self.XBee = XBeeInterface(port,baudRate)
        self.output = output
        self.opFlag = opFlag
        self.comInput = comInput
        self.exitFlag = False
        self.log = open(logFileName,"a");
        self.lastMsg = ""
        self.lastCommand = ""
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
            if result:
                self.lastMsg += result[0]
                for m in [self.lastMsg,result[1:-1]] if len(result) > 1 else [result[0:-1]]:
                    self.log.flush()
                    if m:
                        m = self.__processMessage(m)
                        self.output.put(m) 
                if len(result) > 1:
                    self.lastMsg = result[-1];
    def __processMessage(self,comm:str)->str:
        parts = comm.split('/');
        if parts[0] == str(MSG_TYPES.COMMAND_REQUEST.value):
            self.write(self.lastCommand)
            self.log.write("Got command request")
            self.log.flush()
            return None
        if parts[0] == str(MSG_TYPES.COMMAND_RESPONSE.value):
            self.log.write("Got command response {0}".format(parts[-1]))
            self.log.flush()
            return None
        return "".join(parts[1:]) if len(parts) > 1 else "".join(parts)
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
            self.lastCommand = msg;
        except Exception as e:
            BasicIO.errorLog.put(e)
    def comHandler(self,com):
        try:
                self.write(com)
        except Exception as e:
                BasicIO.errorLog.put(e)
    def setErrorLog(log: Queue):
        BasicIO.errorLog = log
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
        self.log.close();


class MSG_TYPES(Enum):
    TELEMETRY = 0xff
    COMMAND_RESPONSE = 0xef
    COMMAND_REQUEST	= 0xdf

