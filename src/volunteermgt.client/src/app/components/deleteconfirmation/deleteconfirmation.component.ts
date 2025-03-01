import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-deleteconfirmation',
  standalone: false,
  templateUrl: './deleteconfirmation.component.html',
  styleUrl: './deleteconfirmation.component.css'
})
export class DeleteconfirmationComponent {
  constructor(
    public dialogRef: MatDialogRef<DeleteconfirmationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  confirmDelete(): void {
    this.dialogRef.close(true);
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
