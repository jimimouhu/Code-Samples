var obj = {
    fullName: "Matti Makkonen",
    greet: function()  {
        console.log(`Heippa ${this.fullName}`);
    }
}

obj.greet(); // Matti Makkonen
obj.greet.call({fullName: "Maija Makkonen"});
obj.greet.apply({fullName: "Mari Makkonen"});
