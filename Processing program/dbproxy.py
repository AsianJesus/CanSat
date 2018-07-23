import mysql.connector
from cuiinterface import Flag
from threading import Thread
from queue import Queue
from time import sleep

class DBCommand(object):
    comList = {};
    def __init__(self,com:str, args : tuple):
        self.com = DBCommand.comList[com]
        self.args = args
        super().__init__()
    def getCommand(self):
        return self.com
    def getArgs(self) -> tuple:
        return self.args

class DBInterface:
    errorLog: Queue
    def __init__(self,host:str,uid:str,passw:str,schema:str):
        self.host = host
        self.uid = uid
        self.passw = passw
        self.schema = schema
        self.conn = mysql.connector.connect(host=host,user=uid,password=passw,database=schema)
    def close(self)->None:
        self.conn.close()
    def execute(self,command:str,args:tuple) ->tuple:
        cur = self.conn.cursor(True)
        cur.execute(command,args)
        self.conn.commit()
        result = []
        for row in cur:
            result.append(row)
        return tuple(result)
    def isOpen(self)->bool:
        return self.conn.is_connected()

class DataBaseWorker(Thread):
    errorLog : Queue
    def setErrorLog(errorLog:Queue):
        DataBaseWorker.errorLog = errorLog
    def __init__(self, host:str,uid:str,passw:str,schema:str,input:Queue,output:Queue,opFlag: Flag):
        self.conn = DBInterface(host,uid,passw,schema)
        self.input = input
        self.output = output
        self.opFlag = opFlag
        self.exitFlag = False
        super().__init__()
    def start(self):
        self.opFlag.setState(True)
        super().start()
    def stop(self):
        self.opFlag.setState(False)
    def run(self):
        while True:
            if self.exitFlag:
                break
            sleep(1/2)
            if not self.opFlag.getState():
                continue
            try:
                while not self.input.empty():
                    self.__handle(self.input.get())
            except Exception as e:
                DataBaseWorker.errorLog.put(e)
    def __handle(self,cmd):
        if not isinstance(cmd,DBCommand):
            raise TypeError("Invalid type in DatabaseWorker's __hanle")
        result = self.__execute(cmd.getCommand(),cmd.getArgs())
        for row in result:
            self.output.put(row)
    def __execute(self,com:str,args:tuple)->tuple:
        return self.conn.execute(com,args)
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
