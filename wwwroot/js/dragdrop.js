// Funciones para el importador XML de Avital ERP
(function () {
    // Función para hacer clic en elementos ocultos (input file)
    window.clickElement = (element) => {
        if (element) {
            element.click();
        }
    };

    // Función para obtener archivos arrastrados
    window.getDroppedFiles = () => {
        return new Promise((resolve) => {
            // Para drag & drop real, necesitarías implementar lógica adicional
            // Por ahora retorna array vacío
            resolve([]);
        });
    };

    // Función para mostrar alertas
    window.showAlert = (title, message, type = 'info') => {
        alert(`${title}\n${message}`);
    };

    // Inicializar zona de drag & drop
    document.addEventListener('DOMContentLoaded', function () {
        const dropZone = document.getElementById('dropZone');
        if (dropZone) {
            dropZone.addEventListener('dragover', function (e) {
                e.preventDefault();
                e.stopPropagation();
                this.classList.add('dragover');
            });

            dropZone.addEventListener('dragleave', function (e) {
                e.preventDefault();
                e.stopPropagation();
                this.classList.remove('dragover');
            });

            dropZone.addEventListener('drop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                this.classList.remove('dragover');

                // Notificar a Blazor que se soltaron archivos
                if (window.blazorDragDropHandler) {
                    window.blazorDragDropHandler(e);
                }
            });
        }

        // Prevenir comportamientos por defecto
        document.addEventListener('dragover', function (e) {
            e.preventDefault();
        });

        document.addEventListener('drop', function (e) {
            e.preventDefault();
        });
    });
})();
