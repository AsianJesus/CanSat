class Session(object):
    def __init__(self,id:int,name:str):
        self.id = id
        self.name = name
        self.isrunning = False
    def start(self)->None:
        self.isrunning = True
    def end(self)->None:
        self.isrunning = False
    def isRunning(self)->bool:
        return self.isrunning
    def setID(self,id:int)->None:
        if self.id != id:
            self.end()
            self.id = id
    def getID(self)->int:
        return self.id
    def getName(self)->str:
        return self.name
    def stop(self):
        self.isrunning = True
    def setName(self, name:str):
        self.name = name
