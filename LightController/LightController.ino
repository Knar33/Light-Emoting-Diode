int redPin = 3;
int bluePin = 6;
int greenPin = 10;

int red[3] = { 255, 0, 0};
int orange[3] = { 255, 30, 0};
int yellow[3] = { 255, 100, 0};
int green[3] = { 0, 255, 0};
int teal[3] = { 0, 255, 255};
int blue[3] = { 0, 0, 255};
int purple[3] = { 255, 0, 255};

void setup(){
  Serial.begin(9600);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
  pinMode(redPin, OUTPUT);
}

void loop(){
  int received = 0;
  if(Serial.available() == 1)
  {
    received = Serial.read();
    switch (received)
    {
      case 49:     // 1
        ChangeColor(red);
        break;
      case 50:     // 2
        ChangeColor(orange);
        break;
      case 51:     // 3
        ChangeColor(yellow);
        break;   //Move on
      case 52:     // 4
        ChangeColor(green);
        break;
      case 53:     // 5
        ChangeColor(teal);
        break;
      case 54:     // 6
        ChangeColor(blue);
        break;   //Move on
      case 55:     // 7
        ChangeColor(purple);
        break;   //Move on
      default:
        break;
    }
  }
}

void ChangeColor(int color[]){
    analogWrite(redPin, color[0]);
    analogWrite(greenPin, color[1]);
    analogWrite(bluePin, color[2]);
}
