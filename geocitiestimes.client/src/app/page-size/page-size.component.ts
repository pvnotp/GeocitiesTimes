import { Component, output } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-page-size',
  imports: [ReactiveFormsModule],
  templateUrl: './page-size.component.html',
  styleUrl: './page-size.component.css'
})
export class PageSizeComponent {
  
  pageSizeUpdate = output<number>();
  pageSizeControl = new FormControl<number>(15, {
    nonNullable: true,
    validators: [Validators.min(1), Validators.max(50)]
  });

  updatePageSize(n: number) {
    if (this.pageSizeControl.valid) {
      this.pageSizeUpdate.emit(n);
    }
  }
}
