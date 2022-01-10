var person = {
    firstName: 'JokuEtunimi',
    lastName: 'JokuSukunimi',
    greet: function(){

        return this.firstName + ' ' + this.lastName;

    }
}

var matti = Object.create(person);
matti.firstName = "Matti";
matti.lastName = "Meikäläinen";
console.log(matti.greet());
console.log(matti.__proto__.firstName);

var maija = Object.create(matti);
maija.firstName = "Maija";
maija.lastName = "Meikäläinen";
console.log(maija.greet());
console.log(maija.__proto__.__proto__.firstName);
