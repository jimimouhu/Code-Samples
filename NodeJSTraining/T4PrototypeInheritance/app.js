function Person (firstName, lastName){
    this.firstName = firstName;
    this.lastName = lastName;
}

Person.prototype.greet = function() {
    console.log('Maanantai on ihana päivä!' + this.firstName + " " + this.lastName);
}

var matti = new Person('Matti', 'Meikäläinen');
var maija = new Person ('Maija', 'Meikäläinen');
matti.greet();

console.log(matti.__proto__);

console.log(matti.__proto__ === maija.__proto__);