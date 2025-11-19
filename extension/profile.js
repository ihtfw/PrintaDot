class Profile {
    constructor(id = 1, profileName = "default", settings = {}) {
        this.id = id;
        this.profileName = profileName;

        // Main settings
        this.paperHeight = settings.paperHeight ?? 297;
        this.paperWidth = settings.paperWidth ?? 210;
        this.labelHeight = settings.labelHeight ?? 50;
        this.labelWidth = settings.labelWidth ?? 80;
        this.marginX = settings.marginX ?? 2;
        this.marginY = settings.marginY ?? 2;
        this.offsetX = settings.offsetX ?? 0;
        this.offsetY = settings.offsetY ?? 0;
        this.labelsPerRow = settings.labelsPerRow ?? 2;
        this.labelsPerColumn = settings.labelsPerColumn ?? 5;

        // Text settings
        this.textAlignment = settings.textAlignment || 'Left';
        this.textMaxLength = settings.textMaxLength ?? 20;
        this.textTrimLength = settings.textTrimLength ?? 18;
        this.textFontSize = settings.textFontSize ?? 12;
        this.textAngle = settings.textAngle ?? 0;

        // Printer name
        this.printerName = settings.printerName || "default";

        // Barcode type
        this.useDataMatrix = settings.useDataMatrix ?? false;

        // Number settings
        this.numbersAlignment = settings.numbersAlignment || 'Left';
        this.numbersFontSize = settings.numbersFontSize ?? 10;
        this.numbersAngle = settings.numbersAngle ?? 0;

        // Barcode settings
        this.barcodeAlignment = settings.barcodeAlignment || 'Left';
        this.barcodeFontSize = settings.barcodeFontSize ?? 8;
        this.barcodeAngle = settings.barcodeAngle ?? 0;
    }

    toStorageObject() {
        return {
            id: this.id,
            profileName: this.profileName,
            paperHeight: this.paperHeight,
            paperWidth: this.paperWidth,
            labelHeight: this.labelHeight,
            labelWidth: this.labelWidth,
            marginX: this.marginX,
            marginY: this.marginY,
            offsetX: this.offsetX,
            offsetY: this.offsetY,
            labelsPerRow: this.labelsPerRow,
            labelsPerColumn: this.labelsPerColumn,
            textAlignment: this.textAlignment,
            textMaxLength: this.textMaxLength,
            textTrimLength: this.textTrimLength,
            textFontSize: this.textFontSize,
            textAngle: this.textAngle,
            printerName: this.printerName,
            useDataMatrix: this.useDataMatrix,
            numbersAlignment: this.numbersAlignment,
            numbersFontSize: this.numbersFontSize,
            numbersAngle: this.numbersAngle,
            barcodeAlignment: this.barcodeAlignment,
            barcodeFontSize: this.barcodeFontSize,
            barcodeAngle: this.barcodeAngle
        };
    }

    static fromStorageObject(obj) {
        return new Profile(
            obj.id,
            obj.profileName,
            {
                paperHeight: obj.paperHeight,
                paperWidth: obj.paperWidth,
                labelHeight: obj.labelHeight,
                labelWidth: obj.labelWidth,
                marginX: obj.marginX,
                marginY: obj.marginY,
                offsetX: obj.offsetX,
                offsetY: obj.offsetY,
                labelsPerRow: obj.labelsPerRow,
                labelsPerColumn: obj.labelsPerColumn,
                textAlignment: obj.textAlignment,
                textMaxLength: obj.textMaxLength,
                textTrimLength: obj.textTrimLength,
                textFontSize: obj.textFontSize,
                textAngle: obj.textAngle,
                printerName: obj.printerName,
                useDataMatrix: obj.useDataMatrix,
                numbersAlignment: obj.numbersAlignment,
                numbersFontSize: obj.numbersFontSize,
                numbersAngle: obj.numbersAngle,
                barcodeAlignment: obj.barcodeAlignment,
                barcodeFontSize: obj.barcodeFontSize,
                barcodeAngle: obj.barcodeAngle
            }
        );
    }

    static getA4Profile() {
        return new Profile(1, 'A4');
    }

    static getThermoProfile() {
        const settings = {
            paperHeight: 24,
            paperWidth: 45,
            labelHeight: 24,
            labelWidth: 45,
            marginX: 9,
            marginY: 2,
            offsetX: 0,
            offsetY: 0,
            labelsPerRow: 1,
            labelsPerColumn: 1,
            textAlignment: 'Center',
            textMaxLength: 0,
            textTrimLength: 0,
            textFontSize: 12,
            textAngle: 0,
            printerName: "default",
            useDataMatrix: false,
            numbersAlignment: 'Center',
            numbersFontSize: 12,
            numbersAngle: 0,
            barcodeAlignment: 'Center',
            barcodeFontSize: 30,
            barcodeAngle: 0
        };

        return new Profile(2, 'Thermo', settings);
    }
}