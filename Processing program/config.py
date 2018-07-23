import xml.etree.ElementTree as ET
from xml.etree.ElementTree import Element

class XMLIO:
    def __init__(self,filename,rootname):
        self.elements = []
        self.fname = filename
        try:
            self.tree = ET.parse(self.fname)
            self.root = self.tree.getroot()
        except:
            self.root = ET.Element(rootname)
            self.tree = ET.ElementTree(self.root)
    def read(self):
        XMLIO.__expand(self.root,self.elements)
        return self.elements
    def clear(self):
        for elements in self.root:
            self.root.remove(elements)
    def __expand(element:ET.Element,expanded = []):
        if not list(element):
            expanded.append(element)
        else:
            for el in element:
                XMLIO.__expand(el,expanded)
    def flush(self):        
        self.tree.write(self.fname)
    def getElements(self):
        return self.elements
    def getroot(self):
        return self.root

class Configuration:
    def __init__(self,fName):
        self.xml = XMLIO(fName,"config")
        self.isread = False
    def __read(self):
        raw = self.xml.read()
        self.elements = {el.tag : el for el in raw if el.tag != "command"}
        self.commands = {el.attrib["name"] : el for el in raw if el.tag == "command"}
    def get(self,tag):
        if not self.isread:
            self.__read()
        if tag in self.elements:
            return self.elements[tag].text
        else:
            return None
    def set(self,tag,value):
        if not self.isread:
            self.__read()
        if tag in self.elements:
            self.elements[tag].text = value
        elif tag in self.commands:
            self.commands[tag].text = value
        else:
            raise Exception("Tag is not found")
        self.xml.flush()
    def getAllInfo(self):
        if not self.isread:
            self.__read()
        return {key:el.text for key,el in self.elements.items()}
    def getCommands(self):
        if not self.isread:
            self.__read()
        return {key:el.text for key,el in self.commands.items()}
    def saveInfo(self,info: dict,commands:dict):
        self.xml.clear()
        root = self.xml.getroot()
        connInfo = ET.SubElement(root,"connectionInfo")
        for key,value in info.items():
            ET.SubElement(connInfo,key).text = value
        commInfo = ET.SubElement(root,"commands")
        for comName, comBody in commands.items():
            ET.SubElement(commInfo,"command",attrib={"name":comName}).text = comBody
        self.xml.flush()
        