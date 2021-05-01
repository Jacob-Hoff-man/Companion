#include "updateEvent.h"

#ifndef INCLUDE_UPDATEEVENT_CPP
#define INCLUDE_UPDATEEVENT_CPP

#include "CSTExterns.h"

LSM6DS3* IMUEvent::myIMU = (LSM6DS3*)0;

IMUEvent::IMUEvent(BLEService service, int roll) {
#ifdef CST_DEBUG
	Serial.println("Constructing IMUEvent..\t");
#endif
	stat = new Stat(UUID_INCLINE, service, _inclineAngle, roll);
	if (!myIMU) {
		myIMU = new LSM6DS3(I2C_MODE, 0x6A);
		if (!myIMU->begin()) {
#ifdef CST_DEBUG
			Serial.println("\tIMU started");
#endif
		}
		else {
#ifdef CST_DEBUG
			Serial.println("\tIMU failed to start..");
#endif
		}
	}
}

bool IMUEvent::update() {

#ifdef CST_DEBUG
	Serial.println("IMUEvent.update()"); //this verbose debugging is done because arduino lacks the ability to debug.
#endif

	float accelX = myIMU->readFloatAccelX();
	float accelY = myIMU->readFloatAccelY();
	float accelZ = myIMU->readFloatAccelZ();



	inclineAngle = -1 * (atan(accelY / accelZ) / (2 * PI / 360) + RESTING_ANGLE);

#ifdef VALUE_PRINT
	Serial.print("\tincline angle: ");
	Serial.println(inclineAngle, 4);
#endif

	updateTime = successfulUpdateTime = millis();

	updateBLE();

	return true;
}

bool IMUEvent::updateBLE() {
	unsigned long currentTime = millis(); //maybe make this updateTime.
	if (currentTime - characteristicUpdateTime >= characteristicUpdatePeriod) {
		stat->updateStat(inclineAngle);
		return true;
	}
	return false;
}


//IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE////IR-TIRE//


extern float tireDiameter = 26;
float* IRTireEvent::tireDiameterStat = 0;

IRTireEvent::IRTireEvent(BLEService service, int roll) {
#ifdef CST_DEBUG
	Serial.println("Constructing IRTireEvent..\t");
#endif

	stat = new Stat(UUID_DISTANCE, service, _distance, 1); //distance
	statArray[0] = stat;    //distance
	statArray[1] = new Stat(UUID_ACCEL, service, _acceleration, roll);//acceleration
	statArray[2] = new Stat(UUID_SPEED, service, _speed, roll); //speed
	statArray[3] = new Stat(UUID_AVG_SPEED, service, _averageSpeed, 1); //distance/time
	//must define quantity dataPoints Stats or you will get some pointer nonsense going on.


	tireDiameterStat = &tireDiameter;
	prevValue = -1;
	currentSigLength = 0;
	prevSigLength = 0;
	successfulUpdateTime = millis();
	startTime = millis();

	pinMode(IR_TIRE_PIN, INPUT);
	prevValue = digitalRead(IR_TIRE_PIN);
}


bool IRTireEvent::update() {
	char curValue = digitalRead(IR_TIRE_PIN);

	bool returnflag = false;
	if (prevValue && !curValue) { //max measurable speed = 4604/56 (mph)
		if (currentSigLength < 56) {
			currentSigLength = currentSigLength + prevSigLength;
		}
		else {
			updateTime = millis();
			float circumTireInches = PI * *tireDiameterStat;
			float circumTireMeters = circumTireInches / 39.3700787;
			float rotationTime = (updateTime - successfulUpdateTime) / 1000.0;
			//float rpmTire = 60/rotationTime;
			float mpsTire = circumTireMeters / rotationTime;

			distance += circumTireMeters / 1000.0; //km
			acceleration = (mpsTire - speed) / rotationTime;
			speed = mpsTire;
			avgSpeed = distance * 1000 / ((updateTime - startTime) / 1000);

#ifdef VALUE_PRINT
			delay(1);
			//Serial.println("IRTireEvent.update()");
			Serial.print("\n\n\tdistance:\t");
			Serial.println(distance, 4);
			Serial.print("\taccel:\t\t");
			Serial.println(acceleration, 4);
			Serial.print("\tspeed:\t\t");
			Serial.println(speed, 4);
			Serial.print("\tavgSpeed\t");
			Serial.println(avgSpeed, 4);
			Serial.println();
#endif
			updateBLE();

			successfulUpdateTime = updateTime;

			prevSigLength = currentSigLength;
			currentSigLength = 0;
			returnflag = true;
		}
	}
	currentSigLength++;
	prevValue = curValue;

	if (millis() - updateTime > TIRE_TIMEOUT_PERIOD) {

		updateTime = millis();
		float circumTireInches = PI * (*tireDiameterStat);
		float circumTireMeters = circumTireInches / 39.3700787;
		float rotationTime = (updateTime - successfulUpdateTime) / 1000.0;
		float mpsTire = circumTireMeters / rotationTime;

		//distance += circumTireMeters / 1000.0; //km
		acceleration = (mpsTire - speed) / rotationTime;
		speed = mpsTire;
		avgSpeed = distance * 1000 / ((updateTime - startTime) / 1000);

#ifdef VALUE_PRINT
		delay(1);
		Serial.print("\n\n\tdistance:\t");
		Serial.println(distance, 4);
		Serial.print("\taccel:\t\t");
		Serial.println(acceleration, 4);
		Serial.print("\tspeed:\t\t");
		Serial.println(speed, 4);
		Serial.print("\tavgSpeed\t");
		Serial.println(avgSpeed, 4);
		Serial.println();
#endif

		updateBLE();
	}



	return returnflag;
}

bool IRTireEvent::updateBLE() {
	unsigned long currentTime = millis(); //maybe make this updateTime.
	if (currentTime - characteristicUpdateTime >= characteristicUpdatePeriod) {

		float dataSet[dataPoints] = { distance, acceleration, speed, avgSpeed };
		for (int i = 0; i < dataPoints; i++)
			(*statArray[i]).updateStat(dataSet[i]);

		characteristicUpdateTime = currentTime;
		return true;
	}
	return false;
}


//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//GPS//

Adafruit_GPS GPSEvent::GPS = Adafruit_GPS(&Wire);

GPSEvent::GPSEvent(BLEService service) {
#ifdef CST_DEBUG
	Serial.println("Constructing GPSEvent..");
#endif

	statArray[0] = new Stat(UUID_LATITUDE, service, _latitude, 1);
	statArray[1] = new Stat(UUID_LONGITUDE, service, _longitude, 1);
	statArray[2] = new Stat(UUID_ALTITUDE, service, _altitude, 1);

	while (!GPS.begin(0x10)) {

		delay(500);
#ifdef CST_DEBUG
		Serial.println("GPS failed.");
#endif
	}
	delay(1);
#ifdef CST_DEBUG
	Serial.println("GPS started.");
#endif


	GPS.sendCommand(PMTK_SET_NMEA_OUTPUT_RMCGGA); //recommended data with altitude
	GPS.sendCommand(PMTK_SET_NMEA_UPDATE_1HZ);    //1hz update rate
	GPS.sendCommand(PGCMD_ANTENNA);

	unsigned long startDump = millis();

	while (millis() - startDump < 1000) {
		GPS.read();
	}

	updateTime = successfulUpdateTime = millis();
	characteristicUpdateTime = 0;

#ifdef CST_DEBUG
	Serial.println("GPS setup done.");
#endif

}

bool GPSEvent::update() {
#ifdef CST_DEBUG
	Serial.println("GPSEvent.update()");
#endif

	updateTime = millis();

	/*
	if (updateTime - successfulUpdateTime < 1000)
		return false;
		*/

	char c;
	char oldc = 1;
	unsigned long timeStartedReading = millis();
	for (int i = 0; i < 2; i++) {
		while ((c = GPS.read()) != '*' || (!oldc && !c) || millis() - timeStartedReading > 2000) { //read 2 lines. if empty break. or if we being doing this for a while
#ifdef GPS_DEBUG
			Serial.write(c);
			delayMicroseconds(4);
#endif
			oldc = c;
		}
		if (c == '*') {
			for (int k = 0; k < 3; k++) {
				c = GPS.read();
#ifdef GPS_DEBUG
				Serial.write(c);
				delayMicroseconds(4);
#endif
			}

		}
	}

	if (!GPS.newNMEAreceived()) {
#ifdef GPS_DEBUG
		Serial.println("\tNo new NMEA");
#endif
	}
	else {
#ifdef GPS_DEBUG
		Serial.println("\tNew NMEA");
#endif
		if (!GPS.parse(GPS.lastNMEA())) { // this also sets the newNMEAreceived() flag to false
#ifdef GPS_DEBUG
			Serial.println("\tfailed to parse NMEA..");
#endif
		}
		else {

#ifdef GPS_DEBUG
			Serial.println("\tNMEA PARSED");
#endif

			latitude = GPS.latitudeDegrees;
			longitude = GPS.longitudeDegrees;
			altitude = GPS.altitude;

#ifdef VALUE_PRINT
			//if (!GPS.fix) {
			//	return false;
			//}
			delay(1);
			Serial.print("\n\n\tLatitude:\t");
			Serial.println(latitude, 10);
			Serial.print("\tLongitude:\t");
			Serial.println(longitude, 10);
			Serial.print("\tAltitude:\t");
			Serial.println(altitude);
			Serial.println();
#endif

			updateBLE();

			successfulUpdateTime = updateTime;
			return true;
		}
	}
	return false;
}

bool GPSEvent::updateBLE() {
	unsigned long currentTime = millis(); //maybe make this updateTime.
	if (currentTime - characteristicUpdateTime >= characteristicUpdatePeriod) {
		statArray[0]->updateStat(latitude);
		statArray[1]->updateStat(longitude);
		statArray[2]->updateStat(altitude);
		characteristicUpdateTime = currentTime;
		return true;
	}
	return false;
}


#endif
