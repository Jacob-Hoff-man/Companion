# Senior Project Final Demonstration Video
The Cycle Statistic Tool final demonstration was presented on 04/20/21. A YouTube video made by the project team can be viewed with the following link:

https://youtu.be/cTovEaKmmAo
# -----
# Cycle Statistic Tool Companion Mobile Application
Written in C# using the Xamarin platform.

For documentation of the mobile application design, check out the Software Requirements Specifications.pdf file. 
# -----
# Cycle Statistic Tool Hardware
Written in C++.
# -----
Compiling Insructions:

You will need the following devices from the Arduio Device Manager:
	Arduino Nano IoT 33 Core

You will need the following libraries from the Arduino Library Manager:
	ArduinoBLE
	Adafruit_GPS
	Adafruit_EPD
	Adafruit_GFX
	SparkFunLSM6DS3

Delete 'main.cpp' from both the final and secondary microcontroller folders. Those files were placed there only for readability on GitHub. The files named 'final_primary_microcontroller_code.ino' and 'secondary_primary_microcontroller_code.ino' are equivalent to their respective 'main.cpp'.

Place both the /final_primary_microcontroller_code/ folder and /secondary_primary_microcontroller_code/ folder in /Documents/Arduino/

Open the .ino for the microcontroller that you want to program

Compile and upload code.
# -----
Comprehension:

main (primary)
	calls setup functions, polls BLE, and fires event-lists.


CSTDefinitions.h
	defines constants used throughout the code. includes bluetooth ID's, update frequencies, array indices
	
	
CSTExterns.h
	externs
	
	
display.h
	contains all of the code that operates the display. includes setup code and update code. just call updateDisplay(), the function will pull all necessary data.
	
	
PrimaryBLEHandling.h
	contains function for setting up BLE (adding services, characteristics, names etc.). defines handlers that are called by BLE.poll() in the main loop.
	
	
Stat.h
	simple wrapper that updates BLE characteristics and records a rolling avg.
	
	
updateEvent.h
	abstract class that is used to defines what happens when a sensor is updated. updateEvents should also update BLE Stats (and thereby characteristics) when new measurements are taken.
# -----  
