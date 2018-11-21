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

int currentColor[3] = {0, 0, 0};

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

void ChangeColor(int futureColor[]){
  bool changing = true;
  
  while(changing) {
    //change red
    if (futureColor[0] != currentColor[0]) {
      if (futureColor[0] > currentColor[0]) {
        currentColor[0] += 1;
        analogWrite(redPin, currentColor[0]);
      }
      else {
        currentColor[0] -= 1;
        analogWrite(redPin, currentColor[0]);
      }
    }
    
    //change green
    if (futureColor[1] != currentColor[1]) {
      if (futureColor[1] > currentColor[1]) {
        currentColor[1] += 1;
        analogWrite(greenPin, currentColor[1]);
      }
      else {
        currentColor[1] -= 1;
        analogWrite(greenPin, currentColor[1]);
      }
    }
    
    //change blue
    if (futureColor[2] != currentColor[2]) {
      if (futureColor[2] > currentColor[2]) {
        currentColor[2] += 1;
        analogWrite(bluePin, currentColor[2]);
      }
      else {
        currentColor[2] -= 1;
        analogWrite(bluePin, currentColor[2]);
      }
    }

    delay(1);
    
    if (futureColor[0] == currentColor[0] && futureColor[1] == currentColor[1] && futureColor[2] == currentColor[2]) {
      changing = false;
    }
  }
}
