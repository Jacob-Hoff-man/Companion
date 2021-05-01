#ifndef INCLUDE_UPDATEEVENT
#define INCLUDE_UPDATEEVENT

#include "Stat.h"
#include "SparkFunLSM6DS3.h"
#include <Adafruit_GPS.h>
#include "Math.h"
#include "CSTDefinitions.h"

class UpdateEvent {
protected:
	//all implementations of UpdateEvent should probably be singletons
public:
    Stat* stat;
    unsigned long updateTime;
    unsigned long successfulUpdateTime;
	unsigned long characteristicUpdateTime;
	unsigned long characteristicUpdatePeriod = 1000;

    virtual bool update() = 0;
	virtual bool updateBLE();

};

//TODO
//change the following classes to be singletons.

	//primary imu    -->    incline     later maybe:: acceleration, bumpiness
class IMUEvent : public UpdateEvent {
protected:
	static LSM6DS3* myIMU;
public:
	IMUEvent(BLEService Service, int roll);
	virtual bool update();
	virtual bool updateBLE();
	float inclineAngle = 0;
};





//tireIR  -- >  acceleration   speed (rpm)   distance traveled
class IRTireEvent : public UpdateEvent {
protected:
	//pin = TIR
	char prevValue = -1;
	unsigned int currentSigLength = 0;
	unsigned int prevSigLength = 0;
	unsigned long startTime = 0;
	static const int dataPoints = 4;


public:
	IRTireEvent(BLEService Service, int roll = 5);
	virtual bool update();
	virtual bool updateBLE();
	static float* tireDiameterStat; //inches

	Stat* statArray[dataPoints];

	float distance = 0; //km
	float speed = 0;  //m/s
	float acceleration = 0; //m/s*s
	float avgSpeed = 0; //m/s
};

	




class GPSEvent : public UpdateEvent {
protected:

public:
	static Adafruit_GPS GPS;
	GPSEvent(BLEService service);
	virtual bool update();
	virtual bool updateBLE();
	Stat* statArray[3];
	float latitude;
	float longitude;
	float altitude;
};



	//potentiometer
	//	windDirection

	//cpu fan
	//	winddirection
	


	//secondary imu (secondary)
	//	pedalAngularFreq

	//strain gauge (secondary
	//	torque

	//both secondary (secondary)
	//	power
	//	calories burnt

	//pedalAngularFreq(secondary), speed (tire rpm) acceleration
	//	gear ratio

#endif