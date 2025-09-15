class Profile {
    constructor(id = 1, settings = {}) {
        this.id = id;
        
        this.version = 1;
        this.type = "profile";

        this.profileName = settings.profileName || "default";

        // Main settings
        this.paperHeight = settings.paperHeight || 297;
        this.paperWidth = settings.paperWidth || 210;
        this.labelHeight = settings.labelHeight || 50;
        this.labelWidth = settings.labelWidth || 80;
        this.marginX = settings.marginX || 2;
        this.marginY = settings.marginY || 2;
        this.offsetX = settings.offsetX || 0;
        this.offsetY = settings.offsetY || 0;
        this.labelsPerRow = settings.labelsPerRow || 2;
        this.labelsPerColumn = settings.labelsPerColumn || 5;

        // Text settings
        this.textAlignment = settings.textAlignment || 'Center';
        this.textMaxLength = settings.textMaxLength || 20;
        this.textTrimLength = settings.textTrimLength || 18;
        this.textFontSize = settings.textFontSize || 12;
        this.textAngle = settings.textAngle || 0;

        // Barcode type
        this.useDataMatrix = settings.useDataMatrix || false;

        // Number settings
        this.numbersAlignment = settings.numbersAlignment || 'Center';  
        this.numbersFontSize = settings.numbersFontSize || 10;
        this.numbersAngle = settings.numbersAngle || 0;

        // Barcode settings
        this.barcodeAlignment = settings.barcodeAlignment || 'Center';
        this.barcodeFontSize = settings.barcodeFontSize || 8;
        this.barcodeAngle = settings.barcodeAngle || 0;
    }

    // Convert to plain object for storage
    toObject() {
        return {
            id: this.id,
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
            useDataMatrix: this.useDataMatrix,
            numbersAlignment: this.numbersAlignment,
            numbersFontSize: this.numbersFontSize,
            numbersAngle: this.numbersAngle,
            barcodeAlignment: this.barcodeAlignment,
            barcodeFontSize: this.barcodeFontSize,
            barcodeAngle: this.barcodeAngle
        };
    }

    // Create from plain object
    static fromObject(obj) {
        return new Profile(obj.id, obj);
    }

    // Get default settings (id = 0)
    static getDefaultProfile() {
        return new Profile(1);
    }
}