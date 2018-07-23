from threading import Thread
from queue import Queue
from flag import Flag
from session import Session
from time import sleep

class Packet(object):
    def __init__(self,packID:int,recTime:int,flyTime:int,temp:float = None,press:float = None,height:float = None,humidity:float = None,
                 speed:float = None,voltage:float = None,batch:float = None,gpsX:float = None,gpsY:float = None,gpsZ:float = None,
                 co2:float = None,nh3:float = None,no2:float = None):
        self.flyID = None
        self.packID = packID
        self.recTime = recTime
        self.flyTime = flyTime
        self.temperature = temp
        self.pressure = press
        self.height = height
        self.humidity = humidity
        self.speed = speed
        self.voltage = voltage
        self.charge = batch
        self.gpsX = gpsX
        self.gpsY = gpsY
        self.gpsZ = gpsZ
        self.co2 = co2
        self.no2 = no2
        self.nh3 = nh3
        return super().__init__()
    def setFlyID(self,id:int):
        self.flyID = id
    def getInfo(self) -> tuple:
        return (self.flyID,self.packID,self.recTime,self.flyTime,self.temperature,self.pressure,self.height,self.humidity,self.speed,self.voltage,self.charge,self.gpsX,self.gpsY,self.gpsZ,self.co2,self.no2,self.nh3)
    
class DataHandler(Thread):
    errorLog: Queue
    varList = [
        lambda x: "packID" if x > 0 and x < 100 else None,
        lambda x: "recTime" if x > 0 and x < 300 else None,
        lambda x: "flyTime" if x > 0 and x < 300 else None,
        lambda x: "temp" if x > 270 and x < 350 else None,
        lambda x: "press" if x > 95000 and x < 120000 else None,
        lambda x: "height" if x > 0 and x < 600 else None,
        lambda x: "hum" if x > 0 and x < 100 else None,
        lambda x: "speed" if x > -20 and x < 20 else None,
        lambda x: "volt" if x >= 0 and x <= 12 else None,
        lambda x: "charge" if x >= 0 and x <= 100 else None,
        lambda x: "gpsX" if x >= 40 and x <= 60 else None,
        lambda x: "gpsY" if x >= 0 and x <= 1000 else None,
        lambda x: "gpsZ" if x >= 40 and x <= 60 else None,
        lambda x: "co2" if x >= 0 and x <= 10 else None,
        lambda x: "nh3" if x >= 0 and x <= 10 else None,
        lambda x: "no2" if x >= 0 and x <= 10 else None
        ]
    def __init__(self,input:Queue,output:Queue,opFlag:Flag):
        self.input = input
        self.output = output
        self.opFlag = opFlag
        self.exitFlag = False
        super().__init__()
    def start(self):
        self.opFlag.setState(True)
        self.exitFlag = False
        super().start()
    def stop(self):
        self.opFlag.setState(False)
    def __process(raw:str)->Packet:
        elements = raw.split(',')
        variables = {}
        varList = DataHandler.__defineVariables(elements)
        for var in [x for x in DataHandler.varList if x not in variables]:
            variables[var] = None
        result = Packet(packID=variables["packID"],recTime=variables["recTime"],
                        flyTime=variables["flyTime"],temp=variables["temp"],press=variables["press"],
                        height=variables["height"],humidity=variables["hum"],voltage=variables["volt"],
                        batch=variables["charge"],gpsX=variables["gpsX"],gpsY=variables["gpsY"],gpsZ=variables["gpsZ"],
                        co2=variables["co2"],nh3=variables["nh3"],no2=variables["no2"],speed=variables["speed"])
        return result
    def __defineVariables(values)->dict:
        res = {DataHandler.varList[i]:float(values[i](float(values[i]))) for i in range(min(len(values),len(DataHandler.varList))) if DataHandler.__isnumber(values[i])}
        return res
    def __isnumber(n)->bool:
        try:
            float(n)
            return True
        except:
            return False
    def run(self):
        while True:
            if self.exitFlag:
                break
            sleep(1/2)
            if not self.opFlag.getState():
                continue
            while not self.input.empty():
                try:
                    item = self.input.get()
                    if not isinstance(item,str):
                            raise TypeError("Wrong type in processing unit")
                    result = DataHandler.__process(item)
                    self.output.put(result)
                except Exception as e:
                    DataHandler.errorLog.put(e)
    def terminate(self):
        self.exitFlag = True
        self.opFlag.setState(False)
    def setErrorLog(errorLog:Queue):
        DataHandler.errorLog = errorLog
        