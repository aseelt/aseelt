// #include <Arduino.h>
// #include <U8x8lib.h>

//  U8X8_SSD1306_128X64_NONAME_HW_I2C u8x8(/* reset=*/ U8X8_PIN_NONE);

// U8X8_SSD1306_128X64_NONAME_SW_I2C u8x8(/* clock=*/ SCL, /* data=*/ SDA, /* reset=*/ U8X8_PIN_NONE);   // OLEDs without Reset of the Display

// void setup(void) {
//   //u8x8.setBusClock(100000);  // If you breakout other modules, please enable this line
//   u8x8.begin();
//   u8x8.setFlipMode(1);
// }

// void loop(void) {
//   u8x8.setFont(u8x8_font_chroma48medium8_r);
//   u8x8.setCursor(0, 0);
//   u8x8.print("Hello World!");
// }

//Gravity Acceleration
// #include "LIS3DHTR.h"
// #ifdef SOFTWAREWIRE
//     #include <SoftwareWire.h>
//     SoftwareWire myWire(3, 2);
//     LIS3DHTR<SoftwareWire> LIS;       //Software I2C
//     #define WIRE myWire
// #else
//     #include <Wire.h>
//     LIS3DHTR<TwoWire> LIS;           //Hardware I2C
//     #define WIRE Wire
// #endif

// void setup() {
//     Serial.begin(9600);
//     while (!Serial) {};
//     LIS.begin(WIRE, 0x19); //IIC init
//     delay(100);
//     LIS.setOutputDataRate(LIS3DHTR_DATARATE_50HZ);
// }
// void loop() {
//     if (!LIS) {
//         Serial.println("LIS3DHTR didn't connect.");
//         while (1);
//         return;
//     }
//     //3 axis
//     Serial.print("x:"); Serial.print(LIS.getAccelerationX()); Serial.print("  ");
//     Serial.print("y:"); Serial.print(LIS.getAccelerationY()); Serial.print("  ");
//     Serial.print("z:"); Serial.println(LIS.getAccelerationZ());

//     delay(500);
// }



// my own program!!
#include <Arduino.h>
#include <U8x8lib.h>
#include "LIS3DHTR.h"
#ifdef SOFTWAREWIRE
    #include <SoftwareWire.h>
    SoftwareWire myWire(3, 2);
    LIS3DHTR<SoftwareWire> LIS;       //Software I2C
    #define WIRE myWire
#else
    #include <Wire.h>
    LIS3DHTR<TwoWire> LIS;           //Hardware I2C
    #define WIRE Wire
#endif
 U8X8_SSD1306_128X64_NONAME_HW_I2C u8x8(/* reset=*/ U8X8_PIN_NONE);

void setup() {
    //u8x8.setBusClock(100000);  // If you breakout other modules, please enable this line
    u8x8.begin();
    u8x8.setFlipMode(1);Serial.begin(9600);
    while (!Serial) {};
    LIS.begin(WIRE, 0x19); //IIC init
    delay(100);
    LIS.setOutputDataRate(LIS3DHTR_DATARATE_50HZ);
}
void loop() {
    if (!LIS) {
        Serial.println("LIS3DHTR didn't connect.");
        while (1);
        return;
    }
    //3 axis
    u8x8.setFont(u8x8_font_chroma48medium8_r);
    u8x8.setCursor(0, 0);
    float x = LIS.getAccelerationX();
    float y = LIS.getAccelerationY();
    float z = LIS.getAccelerationZ();
    u8x8.print("My position is:\nx: " + String(x) + "\ny: " + String(y) + "\nz: " + String(z));    
    delay(100);
}