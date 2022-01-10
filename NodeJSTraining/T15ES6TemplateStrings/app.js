var firstName = "Matti";
var lastName = "Makkonen";

// Vanha tapa ennen ES6
 var greet = 'Heippa ' + firstName + " " + lastName;
// Uusi tapa
var greet2 = `Heippa ${firstName} ${lastName}`;

console.log(greet);
console.log(greet2);