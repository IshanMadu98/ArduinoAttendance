#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 10
#define RST_PIN 9
MFRC522 mfrc522(SS_PIN, RST_PIN);

#define GREEN_LED 6
#define RED_LED 7

void setup() {
  Serial.begin(9600);
  SPI.begin();
  mfrc522.PCD_Init();

  pinMode(GREEN_LED, OUTPUT);
  pinMode(RED_LED, OUTPUT);

  digitalWrite(GREEN_LED, LOW);
  digitalWrite(RED_LED, LOW);
}

void loop() {
  if (!mfrc522.PICC_IsNewCardPresent() || !mfrc522.PICC_ReadCardSerial()) return;

  String rfid = "";
  for (byte i = 0; i < mfrc522.uid.size; i++) {
    rfid += String(mfrc522.uid.uidByte[i], HEX);
  }

  Serial.println(rfid);  // Send RFID tag to PC/.NET

  // Wait for response (from .NET backend)
  unsigned long startTime = millis();
  while (!Serial.available()) {
    if (millis() - startTime > 3000) { // Timeout: 3 sec
      indicateError();
      return;
    }
  }

  String response = Serial.readStringUntil('\n');

  if (response.indexOf("OK") >= 0) {
    indicateSuccess();
  } else {
    indicateError();
  }

  mfrc522.PICC_HaltA();
  mfrc522.PCD_StopCrypto1();

  delay(1000); // wait before next read
}

void indicateSuccess() {
  digitalWrite(GREEN_LED, HIGH);
  delay(500);
  digitalWrite(GREEN_LED, LOW);
}

void indicateError() {
  digitalWrite(RED_LED, HIGH);
  delay(500);
  digitalWrite(RED_LED, LOW);
}
