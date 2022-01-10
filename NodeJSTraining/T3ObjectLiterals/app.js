// Luodaan objekti suoraan ja funktio jolla luodaan objekti


// Suora objekti
var person = {
    firstName : 'Jimi',
    lastName : 'Mouhu',
    
    
    greet: function(){
        console.log("Hei, toivottavasti ei ole vielä nälkä. " 
        + this.firstName + " " + this.lastName);
    }
}

person.greet();


// funktio objektin luonnille
function Person(etunimi, sukunimi, hetu) {
    this.firstName = etunimi;
    this.lastName = sukunimi;
    this.personID = hetu;
}

var maija = new Person('Maija', 'Meikäläinen', '110994-123K');

console.log(maija.firstName);