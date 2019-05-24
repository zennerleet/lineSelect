int FSR_Pin = A0; //analog pin 0
unsigned char redPin_left = 13; 
unsigned char greenPin_left = 12;
unsigned char redPin_right = 8; 
unsigned char greenPin_right = 7;

void setup(){
  pinMode(redPin_left,OUTPUT);
  pinMode(greenPin_left,OUTPUT);
  pinMode(redPin_right,OUTPUT);
  pinMode(greenPin_right,OUTPUT);
  Serial.begin(9600);
}



void loop(){
  
  int FSRReading = analogRead(FSR_Pin); 
  Serial.println(FSRReading);
  if (FSRReading < 50) {
    digitalWrite(redPin_left, HIGH);
    digitalWrite(greenPin_left, LOW); 
    digitalWrite(greenPin_right, LOW);
    digitalWrite(redPin_right, HIGH); 
   
  }
  else if (FSRReading > 700) {
    digitalWrite(redPin_left, HIGH);
    digitalWrite(greenPin_left, LOW); 
    digitalWrite(greenPin_right, HIGH);
    digitalWrite(redPin_right, LOW); 
  }
  else if (FSRReading > 50 && FSRReading < 700) {
    digitalWrite(redPin_left, LOW);
    digitalWrite(greenPin_left, HIGH); 
    digitalWrite(greenPin_right, LOW);
    digitalWrite(redPin_right, HIGH); 
  }
  delay(1500);
}
