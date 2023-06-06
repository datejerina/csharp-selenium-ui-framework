Feature: Contact

Como usuario de Store Product quiero recibir una notificación 
en caso de que se envía el mensaje satisfactoriamente 
o en caso de que ocurra un error al enviar el formulario de contacto, para poder corregir los datos y volver a intentarlo.

@e2e @smoke
Scenario: Enviar Mensaje de Contacto válido
	Given El usuario ingresa a la pagina Store Product
	When El usuario selecciona la opcion de menu Contact
	And El usuario ingresa el email "daniel@gmail.com" en Contact Email
	And El usuario ingresa el nombre "daniel" en Contact Name
	And El usuario ingresa el mensaje "mensaje de prueba" en Message
	And El usuario presiona el boton Send Message
	Then El usuario recibe el mensaje "Thanks for the message!!"

@e2e @regression
Scenario: Enviar Mensaje de Contacto inválido
	Given El usuario ingresa a la pagina Store Product
	When El usuario selecciona la opcion de menu Contact
	And El usuario ingresa el email "sin dominio" en Contact Email
	And El usuario ingresa el nombre "d@n!$%l" en Contact Name
	And El usuario ingresa el mensaje "m" en Message
	And El usuario presiona el boton Send Message
	Then El usuario recibe el mensaje "Error en el ingreso de datos!!"

@e2e @regression
Scenario Outline: Enviar Mensaje de Contacto valido e inválido
	Given El usuario ingresa a la pagina Store Product
	When El usuario selecciona la opcion de menu Contact
	And El usuario ingresa el email "<email>" en Contact Email
	And El usuario ingresa el nombre "<nombre>" en Contact Name
	And El usuario ingresa el mensaje "<mensaje>" en Message
	And El usuario presiona el boton Send Message
	Then El usuario recibe el mensaje "<mensaje_validacion>"

Examples:
	| email            | nombre  | mensaje | mensaje_validacion             |
	| daniel@gmail.com | daniel  | mensaje | Thanks for the message!!       |
	| sin dominio      | d@n!$%l | m       | Error en el ingreso de datos!! |