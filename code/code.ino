int potPin = 0;
double val = 0;

void setup() {
  Serial.begin(9600);
}

void loop() {
  val = analogRead(potPin);
  val = val / (double)1023.0;

  Serial.println(val);
} 
