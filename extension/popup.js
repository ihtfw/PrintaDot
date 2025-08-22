 document.getElementById('printBtn').addEventListener('click', function() {
            const sampleName = document.getElementById('sampleName').value.trim();
            const barcode = document.getElementById('barcode').value.trim();
            
            if (!sampleName || !barcode) {
                alert('Заполните все поля!');
                return;
            }
            
            printSample(sampleName, barcode);
});