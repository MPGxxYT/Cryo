
# Cryo 
An AES file encryptor and decryptor class library.

Since this is a little coding project for myself, 
no guarantee this is perfect security. 
But I think it's pretty good though.

This class has a built in byte array generator based on passwords. 
It's rudimentary but it works. You don't need to use it if you don't like.
## Byte Array Generator
~~~csharp  
// GenerateBytes(string password, int size);

// password is obvious: the password you'd like to use
// size: is the amount of bytes you'd like to use.
// !!NOTE!! IVs can ONLY BE 16bytes.
// Keys can be 16, 24 or 32bytes.

Processor pros = new();
string password = "MyPassword";

// Generates a 32byte array based on the password.
byte[] Key = pros.GenerateBytes(MyPassword, 32);

// Generates a 16byte array based on the password.
byte[] IV = pros.GenerateBytes(MyPassword, 16);

// These will be used when encrypting and decrypting.
~~~  

## Methods

~~~csharp  
Encrypt(string filePath, byte[] Key, byte[] IV);
Decrypt(string filePath, byte[] Key, byte[] IV);

// filePath: the path to the file you want to encrypt/decrypt
// Key: A 16, 24 or 32 byte array.
// IV: A 16 byte array.
~~~

## Encrypting
~~~csharp  
// Assuming we're using the Key and IV from before.

Processor proc = new();
string filePath = "test.txt";
proc.Encrypt(filePath, Key, IV);

// Simply will encrypt the file.
// It'll output a file with the .enc attached to it.
// ex. test.txt.enc (this is the encrypted file)

~~~  

## Decrypting

~~~csharp  
// Assuming we're using the Keys and IVs from before.

Processor proc = new();
string filePath = "test.txt.enc";
proc.Decrypt(filePath, Key, IV);
// Returns the original file in it's original form.

~~~  
