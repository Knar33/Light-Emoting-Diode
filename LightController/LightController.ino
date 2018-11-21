int greenPin = 10;
int bluePin = 5;
int redPin = 9;

void setup(){
  Serial.begin(9600);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
  pinMode(redPin, OUTPUT);
}

void loop(){
  if(Serial.available() == 1)
  {
    received = Serial.read();
    switch (received)
    {
      case 48:     // 0
        fadeFromTo(bluePin, greenPin);
        break;
      case 49:     // 1
        fadeFromTo(greenPin, redPin);
        break;
      case 50:     // 2
        fadeFromTo(redPin, bluePin);
        break;   //Move on
    }
  }
}

void fadeFromTo(int from, int to){
  for(int i = 0; i < 255; i++){
    analogWrite(to, i);
    delay(5);
    analogWrite(from, 255 - i);
    delay(5);
  }
}
