var obj = {
    greet: "Heippa"
}

console.log(obj.greet);
console.log(obj['greet']);

var prop = 'greet';
console.log(obj[prop]);

 // Taulukot voivat sisältää funktioita sisällään.
 // Jos taulukon kaikki elementit ajetaan, ajetaan kaikki taulukon funktiot

 var arr = [];

 // Laitetaan taulukkoon funktioita array.push():lla 

 arr.push(function() {
     console.log("Moi");
 });
 arr.push(function() {
    console.log("Moi2");
});

 arr.forEach(function(item){
    item(); // ajetaan jokainen funktio vuorollaan taulukosta
 });