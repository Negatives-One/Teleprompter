import mouse
import serial.tools.list_ports
import ctypes

user32 = ctypes.windll.user32
screensize = user32.GetSystemMetrics(0), user32.GetSystemMetrics(1)

TPBoundsY = 0
print(screensize)

height = 1080.0
arduino = 0

def get_ports():

    ports = serial.tools.list_ports.comports()
    
    return ports

def findArduino(portsFound):
    
    commPort = 'None'
    numConnection = len(portsFound)
    
    for i in range(0,numConnection):
        port = foundPorts[i]
        strPort = str(port)
        
        if 'Arduino' in strPort: 
            splitPort = strPort.split(' ')
            commPort = (splitPort[0])

    return commPort
            
                    
foundPorts = get_ports()        
connectPort = findArduino(foundPorts)

if connectPort != 'None':
    arduino = serial.Serial(connectPort,baudrate = 9600, timeout=1)
    arduino.close()
    print('Connected to ' + connectPort)

else:
    print('Connection Issue!')
    
def map_range(value, start1, stop1, start2, stop2):
   return (value - start1) / (stop1 - start1) * (stop2 - start2) + start2

arduino.open()
while (True):
    try:
        
        linha = arduino.readline()
        
        floatValue = float(linha)
        if (floatValue >= 0.88):
            floatValue = map_range(floatValue, 0.88, 1.0, 0.5, 1.0)
        else:
            floatValue = map_range(floatValue, 0.0, 0.88, 0, 0.5)
        print(floatValue)
        intValue = int(float(linha) * screensize[1])
        #print(intValue)
        mouse.move(-1330, intValue)
    except:
        print("Algo deu errado")