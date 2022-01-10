var util = require ("util");

function Person() {
    this.firstName = "Matti";
    this.lastName = "Makkonen";

}

Person.prototype.greet = function() {
     console.log("Heippa " + this.firstName + " " + this.lastName);

}

function PoliceMan() {
    Person.call(this); // Tämä hoitaa perinnän täydellisyyden
    this.badgeNumber = "1234";

}

util.inherits(PoliceMan, Person);
var officer = new PoliceMan();
officer.greet();
//officer.firstName = "Bolise";
//officer.lastName = "Män";
officer.greet();