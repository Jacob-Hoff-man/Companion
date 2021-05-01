#ifndef CST_DISPLAY
#define CST_DISPLAY


void displaySetup();
int updateColumnLocations(int offset = 0);
void printRightCol(const char* name, float value, const char* units, int height);
void printLeftCol(const char* name, float value, const char* units, int height);
void printCol(const char* name, float value, const char* units, int height, int offset);
void updateDisplay();

#endif