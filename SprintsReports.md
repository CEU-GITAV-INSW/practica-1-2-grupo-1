### INFORMACIÓN
3 sprints de 2 semanas:
- 27 Nov a 10 Dic
- 11 Dic a 24 Dic
- 25 Dic a 7 Enero

Sprint planning: Qué issues formaban el sprint
Sprint review y retrospective: Qué se ha conseguido en el sprint y qué no. Qué ha ido bien, qué podría haber mejorado, etc

# SPRINT 1 -> 27 Nov - 10 Dec:
- Sprint Planning: Issues nº: 43 y 48.
    * Nº48 : Se ha implementado la distinción de los números ingresados por el usuario para cada color del tablero. Podría haberse implementado una funcionalidad para que el usuario elija el color con el que desea escribir los números. Al hacer esto también nos hemos dado cuenta de que con ciertos colores no se distinguen bien los números, así que creamos una función para que dependiendo del fondo elegido se cambie también el color a uno que contraste.
    * Nº43 : No da tiempo a realizare, se deja para el siguiente sprint.

# SPRINT 2 -> 10 Dec -  24 Dec:
- Sprint Planning: issues nº 44, 45 y 46
- Sprint review & retrospective:
    * Nº44 : Se ha corregido el problema. Actualmente el botón de mute funciona si has iniciado partida con la música activada.
    * Nº45 : Solucionado, el botón de pausa realmente pausa y reanuda la música desde el punto en el que se pausó.
    * Nº46 : Se ha conseguido arreglar el problema, que era que nunca salía del bucle. Ahora cuando se termina el sudoku o se acaba el tiempo termina y te lo indica. Sin embargo, no se termina de entender por qué el código anterior no funcionaba. Lo óptimo habría sido que en el bucle estuviera incluida la condición de tiempo no acabado.

# SPRINT 3 -> 25 Dic - 7 Ene:
- Sprint Planning: issues nº 43, 47 y 48
- Sprint review & retrospective:
    * Nº43 : Hemos tenido muchas complicaciones a la hora de hacer el game loop. Al ser un código tan largo hecho ya de otra manera ha sido muy tedioso reestructurarlo. Hemos tenido que separar todo en métodos nuevos, crear variables de control gobales para saber en todo momento dónde está el programa... Finalmente se ha conseguido con éxito, aunque sospechamos que no de la mejor manera ya que hay por ejemplo algunos métodos de mostrar por pantalla en el input handle, o otras cosas así mezcladas. Aunque no encontramos otra manera al ser un programa con acciones del usuario y consecuentas tan directas.
    * Nº47 : Fue fácil, únicamente agregar una variable que se ponga en true cuando el tiempo se acabe, y así al terminar el juego se puede diferenciar entre cuando se gana y cuando no.
    * Nº51 : Ahora la música se inicia de forma automática al compilar el código. Se ha solucionado un bug que inciaba varias pistas de audio al pulsar el botón U en el menú de configuración. Todos los botones relacionados con el audio siguen funcionando de la misma forma.
    * Nº58 : Se ha solucionado un error por el que la pantalla parpadeaba al iniciar partida. Esto se debía a que se limpiaba la pantalla en cada iteración.
 
  Al finalizar todo esto nos hemos dedicado a arreglar múltiples bugs que antes no habíamos visto o que no habíamos podido arreglar. Todo bien al final :)

