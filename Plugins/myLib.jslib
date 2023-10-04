mergeInto(LibraryManager.library, {

    Foo: function () {
        window.alert("Foo!");
    },

   Boo: async function () {
         let promise = new Promise(function(resolve, reject) {
               for (var i = 0; i < 10000000; i++) 
               {
                    console.log('Promise '+i);
               }
              console.log('Promise');
              resolve();
            });
   }
});