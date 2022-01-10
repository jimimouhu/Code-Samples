var buf = new Buffer.from("Moi", "utf8");

console.log(buf);

console.log(buf.toString());
console.log(buf.toJSON());
console.log(buf[2]);

buf.write("makkara");
console.log(buf.toString());