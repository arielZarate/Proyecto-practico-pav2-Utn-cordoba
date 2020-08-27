/// <reference path="../front-end/_inicio.html" />
/// <reference path="../front-end/inicio.html" />
/// <reference path="../front-end/inicio.html" />





//modulo principal
//Observar la inyeccion de dependencia de ui.bootstrap y ngRoute para la pagina SPA
var myApp = angular.module('myApp', ['ui.bootstrap','ngRoute']);



//Primera implementacion:servicio
//$timeout es lo mismo que setTimeout en Js
myApp.service('myService', function ($timeout) {
    this.Alert = function (dialogText, dialogTitle) {
        var alertModal = $('<div class="modal fade" id="myModal"> <div class="modal-dialog"> <div class="modal-content"> <div class="modal-header"> <h4 class=" text-dark">' + (dialogTitle || 'Atención') + '</h4> <button type="button" class="close" data-dismiss="modal">&times;</button> </div> <div class="modal-body"><p>' + dialogText + '</p></div><div class="modal-footer"><button type="button" class="btn" data-dismiss="modal">Cerrar</button></div></div></div></div>');
        $timeout(function () { alertModal.modal(100); }); //no le puse tiempo
    };

    this.Confirm = function (dialogText, okFunc, cancelFunc, dialogTitle, but1, but2) {
        var confirmModal = $('<div class="modal fade" id="myModal"> <div class="modal-dialog"> <div class="modal-content"> <div class="modal-header"> <h4>' + dialogTitle + '</h4> <button type="button" class="close" data-dismiss="modal">&times;</button> </div> <div class="modal-body"><p>' + dialogText + '</p></div><div class="modal-footer"><button ID="SiBtn" class="btn" data-dismiss="modal">' + (but1 == undefined ? 'Si' : but1) + '</button><button type="button" ID="NoBtn" class="btn" data-dismiss="modal">' + (but2 == undefined ? 'No' : but2) + '</button></div></div></div></div >');
        confirmModal.find('#SiBtn').click(function (event) {
            if (okFunc)
                okFunc();
            confirmModal.modal('hide');
        });
        confirmModal.find('#NoBtn').click(function (event) {
            if (cancelFunc)
                cancelFunc();
            confirmModal.modal('hide');
        });
        $timeout(function () { confirmModal.modal(); });
    };
    // bloqueo / desbloqueo de pantalla
    // https://www.w3schools.com/bootstrap4/bootstrap_modal.asp
    // https://www.w3schools.com/bootstrap4/bootstrap_progressbars.asp
    var contadorBloqueo = 0;
    var $dialog = $(`
   <div class="modal" id="myModal">
    <div class="modal-dialog">
      <div class="modal-content">
      <!-- Modal Header -->
        <div class="modal-header">
          <h5 class="modal-title">Espere por favor...</h5>
        </div>
       <!-- Modal body -->
        <div class="modal-body">
	        <div class="progress">
        		  <div class="progress-bar progress-bar-striped progress-bar-animated" style="width:100%">
    	     </div>	
        </div>
      </div>
    </div>
  </div>
`);
    this.BloquearPantalla = function () {
        contadorBloqueo++;
        if (contadorBloqueo == 1)
            $dialog.modal({
                backdrop: 'static',
                keyboard: false
            });
    };
    this.DesbloquearPantalla = function () {
        contadorBloqueo--;
        if (contadorBloqueo == 0)
            $timeout(function () { $dialog.modal('hide'); }, 100); //dentro de un timeout para que angular actualice la pantalla
    };
});


//segunda implementacion:Interceptor va despues de servicio ya que usa myService
myApp.factory('myHttpInterceptor', function ($q, myService) {
    // factory retorna un objeto
    var myHttpInterceptor = {
        request: function (config) {
            myService.BloquearPantalla();
            return config;
        },
        requestError: function (config) {
            return config;
        },
        response: function (response) {
            myService.DesbloquearPantalla();
            return response;
        },
        responseError: function (response) {
            myService.DesbloquearPantalla();
            // acceso denegado generado por alguna llamada al servidor (no carga las vistas)
            if (response.status == 404 || response.status == 401) {
                myService.Alert("Acceso Denegado...");
            }
            else if (response.status == 400) {
                myService.Alert("Peticion Incorrecta...");
            }
            else if (response.data&&response.data.ExceptionMessage) {
                // error desdewebapi
                myService.Alert(response.data.ExceptionMessage);
            }
            else {
                myService.Alert("Error en la aplicacion, reintente nuevamente.");
            }
            return $q.reject(response);
        }
    }
    return myHttpInterceptor;
});





//configura la app con el interceptor antes creado


myApp.config(function ($httpProvider, $routeProvider) {
    $httpProvider.interceptors.push('myHttpInterceptor');

    $routeProvider.caseInsensitiveMatch = true;  // no sensitivo mayusculas/minusculas
    // ref angular ruteo; configuracion del ruteo del cliente
    $routeProvider.

when('/inicio',  // segun el valor del atributo href del menu, cambiando #! por /
        {
            //../front-end/_inicio.html
            templateUrl: '../front-end/_inicio.html',
            controller: 'InicioCtrl'
            //            controller: 'InicioCtrl'  // controlador de angular al cual se hace referencia en la vista (ng-controller)
        }).
when('/empleado', // segun el valor del atributo href del menu, cambiando #! por /
        {
            templateUrl: '../front-end/_empleado.html',
            controller: 'EmpleadoCtrl'  // controlador de angular al cual se hace referencia en la vista (ng-controller)
        }).
when('/departamentos', // segun el valor del atributo href del menu, cambiando #! por /
        {
            templateUrl: '../front-end/_departamentos.html',
            controller: 'DepartamentoCtrl'
        }).
when('/acerca_de', // segun el valor del atributo href del menu, cambiando #! por /
        {
            templateUrl: '../front-end/_acerca_de.html',
            controller: 'InicioCtrl'
        }).
when('/contacto', // segun el valor del atributo href del menu, cambiando #! por /
        {
            templateUrl: '../front-end/_contacto.html',
            controller: 'InicioCtrl'
        }).
otherwise(
        {
            redirectTo: '../front-end/_inicio.html'  //si no se encuentra la ruta en los when anteriores lo redirige a /inicio
            
        });
});


// tengo un objeto $rootScope padre de todos

myApp.run(function ($rootScope, $http, $location, myService) {  
    // $rootScope desde donde heredan todos los $scope de los controladores
    // todas las variables o funciones que se definan aquí están disponibles en todos los controladores
    $rootScope.TituloAccionABMC = { A: '(Agregar)', B: '(Eliminar)', M: '(Modificar)', C: '(Consultar)', L: '(Listado)' };
    $rootScope.AccionABMC = 'L';   // inicialmente inicia elel listado (buscar con parametros)
    $rootScope.Mensajes = { SD: ' No se encontraron registros...', RD: ' Revisar los datos ingresados por favor...' };
});




//controlador Inicio

myApp.controller('InicioCtrl', function ($scope) {
    $scope.Titulo= 'RRHH Integral';
});


//Controlador Departamentos

myApp.controller('DepartamentoCtrl',

   function ($scope, $http, myService) {


       $scope.Titulo2 = 'Gestionar Departamentos';

       //myService.BloquearPantalla();
       $http.get('/api/Departamentos')
           .then(function (response) {
               $scope.Lista = response.data;

           
               //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente
           }); 
   }  
);




//Controlador Empleado
 //myApp.controller('EmpleadoCtrl',
 //    function ($scope, $http) {
 //        $scope.Titulo = 'Gestionar Empleados';  // inicia mostrando el Listado
 //        // articulos cargados inicialmente, como demo para probar la interface visual
 //        $scope.Lista = [{ IdArticulo: 1, Nombre: 'Ariel', Apellido: 'Zarate', Sueldo: 110.25, FechaAlta: '18/03/2019', IdDpto: 10, Activo:false }, { IdArticulo: 2, Nombre: 'Jose', Apellido: 'Zabala', Sueldo: 300.25, FechaAlta: '28/08/2011', IdDpto:12, Activo: true }];
 //    }
 //);

myApp.controller('EmpleadoCtrl',
   function ($scope, $http,myService) {

       $scope.Titulo3 = 'Gestionar Empleados';  // inicia mostrando el Listado
       // articulo cargado inicialmente, como demo para probar la interface visual (luego comentar esta linea)
      
       //$scope.Lista = [{ Id: 1, Nombre: 'Ariel', Apellido: 'Zarate', Sueldo: 110.25, FechaAlta: '18/03/2019', IdDpto: 10, Activo: false }, { Id: 2, Nombre: 'Jose', Apellido: 'Zabala', Sueldo: 300.25, FechaAlta: '28/08/2011', IdDpto: 12, Activo: true }];


       //   QUEDAN SUPLANTADAS POREL rootScope
     //  $scope.TituloAccionABMC = { A: '(Agregar)', B: '(Eliminar)', M: '(Modificar)', C: '(Consultar)',  L: null };
      // $scope.AccionABMC = 'L';   // inicialmente inicia el el listado (buscar con parametros)
      // $scope.Mensajes = { SD: ' No se encontraron registros...', RD: ' Revisar los datos ingresados...' };


       $scope.DtoFiltro = {};    // dto con las opciones para buscar en grilla
       $scope.DtoFiltro.Activo = null;
       $scope.PaginaActual = 1;  // inicia pagina 1





       // opciones del filtro activo
       $scope.OpcionesSiNo = [{ Id: null, Nombre: '' }, { Id: true, Nombre: 'SI' }, { Id: false, Nombre: 'NO' }];



       // invoca metodo WebApi para cargar una lista de datos (familias de articulos) que se usa en un combo
       $http.get('/api/Departamentos').then(function (response) {
           $scope.DepartamentoCbo = response.data;
       });



       ///**FUNCIONES**///



       $scope.Agregar = function () {
           $scope.AccionABMC = 'A';
           $scope.DtoSel = {};
           $scope.DtoSel.activo = true;
           $scope.FormReg.$setUntouched();
           $scope.FormReg.$setPristine();  // restaura FormReg.$submitted = false

       };

       

       $scope.Buscar = function () {
           // las propiedades del params tienen que coincidir con el nombre de los parámetros de c# (case sensitive)
           params = {Nombre: $scope.DtoFiltro.Nombre, Activo: $scope.DtoFiltro.Activo, numeroPagina: $scope.PaginaActual };

           //myService.BloquearPantalla();

           $http.get('/api/Empleados', {params: params })
               .then(function (response) {
                   $scope.Lista = response.data.Lista;  // variable para luego imprimir
                   $scope.RegistrosTotal = response.data.RegistrosTotal;  // var para mostrar en interface

                   //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente
               },
            function (response) {
            //myService.DesbloquearPantalla();  // cuando la fn termina por error
         myService.Alert("Error al traer los datos!");
        });
       };

               



       $scope.Consultar = function (dto) {
           //myService.BloquearPantalla();
           $scope.BuscarPorId(dto, 'C');
           //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente
       };



       //comienza la modificacion, luego la confirma con el metodo Grabar
       $scope.Modificar = function (dto) {
           //myService.BloquearPantalla();
           if (!dto.activo) {
               //alert("No puede modificarse un registro Inactivo.");
               myService.Alert("No puede modificarse un registro Inactivo.");
               return;
           }
           $scope.BuscarPorId(dto, 'M');

           //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente
           $scope.FormReg.$setUntouched();
           $scope.FormReg.$setPristine();  // restaura FormReg.$submitted = false

       };





       ////*************BUSCARPORID*******************
   
       $scope.BuscarPorId = function (dto, AccionABMC) {
           $http.get('/api/Empleados/' + dto.id_emple)
               .then(function (response) {
                   $scope.DtoSel = response.data;


                   //convertir fecha de formato ISO 8061 a fecha de javascript para el datepicker
                   $scope.DtoSel.fecha_alta = new Date($scope.DtoSel.fecha_alta);
                   $scope.AccionABMC = AccionABMC;

                   ////formatear fecha de  ISO 8061 a string dd/MM/yyyy
                   //var arrFecha = $scope.DtoSel.fecha_alta.substr(0, 10).split('-');
                   //$scope.DtoSel.fecha_alta = arrFecha[2] + '/' + arrFecha[1] + '/' + arrFecha[0];
                   //$scope.AccionABMC = AccionABMC;

               });
       };

     





       //***********************************************************+

       //grabar tanto altas como modificaciones
       
  
       $scope.Grabar = function () {
           //convertir fecha de string dd/MM/yyyy a ISO para que la entienda webapi
           //var arrFecha = $scope.DtoSel.fecha_alta.substr(0, 10).split('/');
           //if (arrFecha.length == 3)
           //    $scope.DtoSel.fecha_alta = new Date(arrFecha[2], arrFecha[1] - 1, arrFecha[0]).toISOString();

           //myService.BloquearPantalla();
           if ($scope.DtoSel.id_emple == undefined)  // agregar
           {
               $http.post('/api/Empleados/',$scope.DtoSel).then(function (response) {

                   $scope.Volver();
                   $scope.Buscar(); // vuelve a cargar los artículos desde el servidor

                   //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente

                   myService.Alert("Registro agregado correctamente.");
               });
           }
               //actualiza  recibe dos parametros uno el id del empleado del dtoSel y otro Objeto Empleado que se comparara
           else {
               $http.put('/api/Empleados/' + $scope.DtoSel.id_emple, $scope.DtoSel).then(function (response) {
                   $scope.Volver();
                   $scope.Buscar(); // vuelve a cargar los artículos desde el servidor
                   myService.Alert("Registro modificado correctamente.")
               });
           }
       };





       //************ACTIVAR DESACTIVAR*********


       //baja Logica
       $scope.ActivarDesactivar = function (dto) {
        
           myService.Confirm("Esta seguro de " + (dto.activo ? "Desactivar" : "Activar") + " este registro?", fun, null, "Confirmación", "Aceptar", "Cancelar")
              
           function fun() {
               //myService.BloquearPantalla();
               $http.delete('/api/Empleados/' + dto.id_emple, dto).then(function () {
                   //myService.DesbloquearPantalla();  // cuando la fn termina exitosamente
                   $scope.Buscar();
               });
              
           }
       };

    
    

       // Volver Agregar/Modificar
       $scope.Volver = function () {
          1111
           $scope.AccionABMC = 'L';
           //$scope.Buscar(); // vuelve a cargar los artículos desde el servidor

         
           //$scope.FormReg.$setUntouched();
           //$scope.FormReg.$setPristine(); 
       };




       $scope.ImprimirListado = function () {
           myService.Alert("Sin desarrollar...")
       };


   });




       