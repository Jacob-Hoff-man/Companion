#ifndef CST_DEFS
#define CST_DEFS

//ENABLE SERIAL PRINTING
//#define VALUE_PRINT
//#define CST_DEBUG
//#define STAT_DEBUG //crashes fo some reason.. just dont try.
//#define GPS_DEBUG
#define BLE_DEBUG

//PIN DEFINITIONS
#define IR_TIRE_PIN 4

#define EPD_CS      5
#define EPD_DC      6
#define SRAM_CS     7
#define EPD_RESET   8 // can set to -1 to share with RST
#define EPD_BUSY    9 // can set to -1 to not use a pin (will wait a fixed delay)
#define EPD_ENABLE  10  


#define RESTING_ANGLE 25.0



//UPDATE PERIODS
#define ROLLING_AVERAGE_POINTS 32
#define DISPLAY_UPDATE_PERIOD 1000 * 30 * 1 //minutes
#define SLOW_STAT_UPDATE_PERIOD (DISPLAY_UPDATE_PERIOD/ ROLLING_AVERAGE_POINTS) //msec
#define	QUICK_STAT_UPDATE_PERIOD  5

#define TIRE_TIMEOUT_PERIOD 10000


//STAT CONSTANTS
#define _dontCare		0
#define _inclineAngle	1
#define _latitude		2
#define _longitude		3
#define _altitude		4
#define _distance		5
#define _acceleration	6
#define _speed			7
#define _averageSpeed	8
#define _calories		9
#define _power			10
#define _cadence		11
#define _torque			12
#define _gearRatio		13

//minimum torque for updating gear ratio.
#define TORQUE_MIN	1.0
#define INCLINE_MIN	-2.0



//BLUETOOTH CONSTANTS
#define PEDAL_ADDRESS "24:62:ab:b9:69:ee"//"31:82:7d:69:d5:09"

#define UUID_CST_SERVICE	"f5806ca8-6c2f-4f9d-a232-b43b200c9479"
#define UUID_GPS_SERVICE	"b6eb07bd-1079-4a61-ab37-f18b73af22d2"
#define UUID_PEDAL_SERVICE	"732ca8ca-b303-4e56-974a-bb642d11bb3b"

#define UUID_SPEED		"08cdf8af-d030-43ed-85ea-24d020556e21"
#define UUID_DISTANCE	"e4e8f3d1-38f6-48ac-b3a2-a9b8987d6947"
#define UUID_AVG_SPEED	"e76d7d76-5a7a-408f-9844-2dd6eabbc8e6"
#define UUID_ACCEL		"3e29dd5a-c12a-4ccb-b34c-f9d7d6bfbc47"
#define UUID_INCLINE	"fe3a2d74-a83b-40d9-81da-49986812c7d2"
#define UUID_LATITUDE	"5af0c5fa-43f6-42f1-a749-a4a497dde66e"
#define UUID_LONGITUDE	"81d6828c-ddf2-48bc-a3c8-7382336926ef"
#define UUID_ALTITUDE	"100e4627-3e40-437d-b9ca-c63d89b6348f"
#define UUID_POWER		"a1a882a8-dd89-4c86-99d3-b36045fa280d"
#define UUID_CALORIES	"9c90a5cb-02d1-4f56-bf86-7a65f197074a"
#define UUID_GEAR_RATIO "de75328d-63fb-4307-b993-c7e240a01b5a"
#define UUID_CADENCE	"6e072e2b-87d7-4df9-b3e0-02c5f9b4d8a9"
#define UUID_WIND_SPEED "c3092ab4-3b63-4090-a6c7-d6ddc4afd8f0"
#define UUID_WIND_DIR	"3d55cdfc-493f-484a-aa64-0ad499b99c0a"
#define UUID_TORQUE		"e8c8136c-80d3-4450-9b4d-619647eb6314"


#endif
