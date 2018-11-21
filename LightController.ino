int greenPin = 10;
int bluePin = 5;
int redPin = 9;

void setup(){
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
  pinMode(redPin, OUTPUT);
}

void loop(){
    fadeFromTo(bluePin, greenPin);
    fadeFromTo(greenPin, redPin);
    fadeFromTo(redPin, bluePin);
}

void fadeFromTo(int from, int to){
  for(int i = 0; i < 255; i++){
    analogWrite(to, i);
    delay(5);
    analogWrite(from, 255 - i);
    delay(5);
  }
}
