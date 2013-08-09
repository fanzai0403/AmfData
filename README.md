= AmfData

simple code to encode/decode the amf0/amf3 data

== Supported Types

Amf3 - decode : 12 of 18 types, without MovieClip, Reference, Date, Recordset, XML, TypedObject
Amf3 - encode : null, numbers(byte, int, uint, float, double), bool, string, Array. CMixArray, IDictionary.

Amf3 - decode : 9 of 13 types, without XmlDoc, Date, Xml, ByteArray
Amf3 - encode : null, numbers(byte, int, uint, float, double), bool, string, Array, IDictionary.

== Reference

Amf0: http://osflash.org/documentation/amf/astypes
Amf3: <<Action Message Format - AMF 3>>
Amf3: https://code.google.com/p/amf3cplusplus/  (this code has bugs)

