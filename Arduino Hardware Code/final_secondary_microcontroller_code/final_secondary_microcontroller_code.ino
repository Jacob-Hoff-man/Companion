#if 1

//24:62:ab:b9:69:ee

#define NANO_IOT

#include <SparkFunLSM6DS3.h>


#include <ArduinoBLE.h>
#include "HX711.h"
#include "RollAvg.h"


#define UUID_CST_SERVICE "732ca8ca-b303-4e56-974a-bb642d11bb3b"

#define UUID_POWER    "a1a882a8-dd89-4c86-99d3-b36045fa280d"
#define UUID_CALORIES "9c90a5cb-02d1-4f56-bf86-7a65f197074a"
#define UUID_CADENCE  "6e072e2b-87d7-4df9-b3e0-02c5f9b4d8a9"
#define UUID_TORQUE     "e8c8136c-80d3-4450-9b4d-619647eb6314"


#define LOADCELL_DOUT_PIN  2
#define LOADCELL_SCK_PIN   3

HX711 scale;

#define CALCULATION_PERIOD 100
#define BLE_UPDATE_PERIOD 2000

#define AVG_LEN 20

#define PEDAL_SHAFT_LENGTH .175 //meters
#define constant 15228.71


LSM6DS3 myIMU(I2C_MODE, 0x6A);

//init as millis so you dont get a crazy big energy calculation
unsigned long lastTimeMeasurementsUpdated = millis();
unsigned long timed = 0;
unsigned long BLEUpdateTime = 0;
unsigned long count = 0;

RollAvg torqueAv(AVG_LEN);
RollAvg powerAv(AVG_LEN);
RollAvg cadenceAv(AVG_LEN);

//not transmit
float mass = 0;
float energySpent = 0;
float angVel = 0;

//transmit but no avg
float calories = 0;

//transmit with avg
float power = 0;
float cadence = 0;
float torque = 0;

//protodypes

void blePeripheralDisconnectHandler(BLEDevice central);
void blePeripheralConnectHandler(BLEDevice central);
void startBLE();
void startScale();
void printMeasurements();
void updateMeasurements();

void setup() {
    Serial.begin(115200);
    //while (!Serial);
    Serial.println("starting secondary microcontroller");

    
    Serial.println("Starting IMU");


    #ifdef NANO_IOT
    myIMU.begin();
    #endif
    
    Serial.println("Starting HX711");
    startScale();
    
    Serial.println("Starting IMU");
    startBLE();

    Serial.println(BLE.address());

}


void loop() {
    delay(10);
    updateMeasurements();
    BLE.poll();


    if (millis() - timed > 5000) {
        Serial.println("Waiting for connection..");
        timed = millis();
    }

    BLEDevice central = BLE.central();
    if (central.connected()) {

        //not used. could remove.
        BLEService CSTService = central.service(UUID_CST_SERVICE);
        if (!CSTService) {
            Serial.println("couldnt find service");
            return;
        }

        BLECharacteristic powerChar = central.characteristic(UUID_POWER);
        delayMicroseconds(30);
        BLECharacteristic caloriesChar = central.characteristic(UUID_CALORIES);
        delayMicroseconds(30);
        BLECharacteristic cadenceChar = central.characteristic(UUID_CADENCE);
        delayMicroseconds(30);
        BLECharacteristic torqueChar = central.characteristic(UUID_TORQUE);
        delayMicroseconds(30);

        while (central.connected()) {
            delay(10);
            updateMeasurements();
            BLE.poll(); //poll after measurement update incase central disconnects while measuring

            if (millis() - BLEUpdateTime > BLE_UPDATE_PERIOD) {
                Serial.println("Writing BLE data");

                //Arduino BLE doesn't let you write floats (even though the characteristic is float type)
                //My solution is to convert to unsigned long (i think compiler casts it to uint8_t afterward)
                float powerF = powerAv.rollingAvg();//*((unsigned long*)&powerF)
                float cadenceF = cadenceAv.rollingAvg();

                float torqueF = torqueAv.rollingAvg();

                powerChar.writeValue(*((unsigned long*)&powerF));
                caloriesChar.writeValue(*((unsigned long*)&calories)); //doesnt get AVG
                cadenceChar.writeValue(*((unsigned long*)&cadenceF));
                torqueChar.writeValue(*((unsigned long*)&torqueF));
               
                BLEUpdateTime = millis();
            }
        }

    }



}


void updateMeasurements() {
    if (millis() - lastTimeMeasurementsUpdated < CALCULATION_PERIOD)
        return;
    count++;

    float x,y,z;


    z = myIMU.readFloatGyroZ();
    
    angVel = abs(((z / 360.0) * 2.0 * PI) / 3.0);
    delay(5);
    
    z = myIMU.readFloatGyroZ();

    angVel += abs(((z / 360.0) * 2.0 * PI) / 3.0);
    mass = abs(scale.get_units());
    torque = mass * PEDAL_SHAFT_LENGTH;

    delay(5);

    z = myIMU.readFloatGyroZ();
    
    angVel += abs(((z / 360.0) * 2.0 * PI) / 3.0);

    power = angVel * torque * 2;

    unsigned long thisTime = millis();
    energySpent += power * (thisTime - lastTimeMeasurementsUpdated) / 1000.0;
    lastTimeMeasurementsUpdated = thisTime;

    calories = energySpent / 4184.0;

    cadence = angVel / (2.0 * PI) * 60;



    torqueAv.updateAvg(torque);
    powerAv.updateAvg(power);
    cadenceAv.updateAvg(cadence);

    printMeasurements();
}



void printMeasurements() {
    Serial.print(count);

    const int decimals = 3;
    Serial.print("\tmass\t");
    Serial.print(mass, decimals);

    Serial.print("\ttorque\t");
    Serial.print(torque, decimals);

    Serial.print("\tcadence\t");
    Serial.print(cadence, decimals);

    Serial.print("\tangVel\t");
    Serial.print(angVel, decimals);

    Serial.print("\tpower\t");
    Serial.print(power, decimals);

    Serial.print("\tJoules\t");
    Serial.print(energySpent, decimals);

    Serial.print("\tCalories\t");
    Serial.print(calories, decimals);

    Serial.println();


    /*
    Serial.print("\ttorque\t");
    Serial.print(torqueAv.rollingAvg(), decimals);

    Serial.print("\tcadence\t");
    Serial.print(cadenceAv.rollingAvg(), decimals);


    Serial.print("\tpower\t");
    Serial.print(powerAv.rollingAvg(), decimals);

    Serial.println();
    */
}



void startScale() {

    scale.begin(LOADCELL_DOUT_PIN, LOADCELL_SCK_PIN);

    scale.set_gain(128);
    scale.set_scale();
    scale.tare();

    scale.set_scale(constant);
}

void startBLE() {

    BLE.begin();
    BLE.setLocalName("CST : Secondary");


    BLE.setEventHandler(BLEConnected, blePeripheralConnectHandler);
    BLE.setEventHandler(BLEDisconnected, blePeripheralDisconnectHandler);

    BLE.advertise();
    Serial.print("advertising\t");
    Serial.println(BLE.address());
}

void blePeripheralConnectHandler(BLEDevice central) {
    
    // central connected event handler
    Serial.print("Connected event, central: ");
    Serial.println(central.address());
    delay(1);
    central.discoverAttributes();
}

void blePeripheralDisconnectHandler(BLEDevice central) {
    // central disconnected event handler
    Serial.print("Disconnected event, central: ");
    Serial.println(central.address());
}
#endif
