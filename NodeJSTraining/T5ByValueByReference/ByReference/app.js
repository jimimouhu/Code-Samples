// By Value

var personName = "Matti";
var firstName = personName;

personName = "Maija";

console.log(personName);
console.log(firstName);

// By Reference

var myName = {
    firstName: "Jimi",
    lastName: "Mouhu"
};

var person = myName;
myName.firstName = "Jani";
console.log(myName.firstName);
console.log(person.firstName);