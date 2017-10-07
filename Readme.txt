All rights reserved to Erez Zinman 2017.

This is a C# code for parsing and creating TL1 (Transaction Layer 1) messages and responses.

About TL1 (From Wikipedia):
	"Transaction Language 1 (TL1) is a widely used management protocol in telecommunications. 
It is a cross-vendor, cross-technology man-machine language, and is widely used to manage optical 
(SONET) and broadband access infrastructure in North America. TL1 is used in the input and output 
messages that pass between Operations Support Systems (OSSs) and Network Elements (NEs). Operations 
domains such as surveillance, memory administration, and access and testing define and use TL1 
messages to accomplish specific functions between the OS and the NE. TL1 is defined in Telcordia 
Technologies (formerly Bellcore) Generic Requirements document GR-831-CORE."

											- https://en.wikipedia.org/wiki/Transaction_Language_1

About the implementation: 
	I didn't have the official specification by Telcordia Technologies, so I used alternative (open)
	sources, as described below in the 'Bibliography' section.

	I tried to make this implementation as general as possible, so I do not implement any 
	vendor-specific behavior, as well as any specific request or response. I DO, however, expose
	a nice set of classes that will allow you to easily implement these behaviors using C# (not 
	unlike an SNMP client that has none of the MIB files precomplied into its source).

	Also, I used Regular Expressions for validations and parsing, so (even though I used the 
	'Compiled' behaviour) it may affect performance, and this assembly might not handle large loads.

	This implementation does not contain the network layer (TCP socket handling, request-response 
	matching), but only the request/response parsing and string.
	
License:
	This assembly is licensed under the MIT license (as described in 'License.txt').
	In addition, I request that you don't take credit for my work, and that if you have corrections,
	please send them to me.

Usage:
	For an overview of the protocol, see the bibliography section.

	Creating a TL1 outgoing message:
		You can either use the "TL1Request" class, or inherit from it in case your vendor has a specific 
	protocol for the payload. In the latter case, you should override the "GetAdditionalDataString" 
	method of the request in the inherited type. If you don't want to inherit, you can add 
	"TL1RequestDataBlock"s to the "AdditionalDataBlocks" property of request.
	An example of inheritance of the TL1Request is "TL1CommonRequest" which adds common functionality to
	the request (common payload-blocks, and common verbs).

	Parsing an incoming message:
		To parse an incoming message use the method "TL1ReceivedMessage.Parse(...)". This method will 
	receive a Stream or a string, and will return either a response, an autonomous message, or an 
	acknowledgement message. 
	The generic variants of this method are in case you decide to implement custom parsing for your
	vendor's protocol. In that case, you should inherit "TL1Response" and "TL1AutonomousMessage",
	as well as create a new Enum to replace "TL1AcknowledgementCodes" as the enum used in
	"TL1AutonomousMessage" (which should not be inherited). 
	The non-generic variants of the "Parse" method are equivalent to:
		"Parse<TL1Response, TL1AutonomousMessage, TL1AcknowledgementCodes>(reader)"
	When deriving "TL1Response" and/or "TL1AutonomousMessage", you should override the
	"ParseAdditionalData" method to implement parsing of vendor-specific information. That method 
	returns a collection of "TL1DataLine"s, which is a data-type that represents lines of data sent by
	the NE. You could also inherit from "TL1DataLine", and create data lines that will externalize 
	line-specific data.
	An example of inheritance of the TL1Response is "TL1CommonResponse" which adds common functionality to
	the Response (common error-format handling for DENY and PRTL responses, as well as comment handling).

Bibliography:
	To create this library, I used data from the following websites:
	1. https://en.wikipedia.org/wiki/Transaction_Language_1
	2. https://www.webnms.com/cagent/help/technology_used/c_tl1overview.html
	3. http://www.dpstele.com/pdfs/white_papers/essential_tl1_guide.pdf
	4. https://www.force10networks.com/CSPortal20/KnowledgeBase/DOCUMENTATION/T-CA-W_Docs/TransNav/TransNav/800-0009-TN31_TransNav_TL1_Reference_Guide.pdf