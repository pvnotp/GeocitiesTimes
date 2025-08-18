import { Component, output } from '@angular/core';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-search',
  imports: [ReactiveFormsModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css'
})
export class SearchComponent {

  searchTermUpdate = output<string | null>();
  searchControl = new FormControl<string>('', {
    nonNullable: true,
    validators: [Validators.minLength(2)]
  });

  triggerSearch(raw: string) {
    // Mark as touched so errors show
    this.searchControl.markAsTouched();

    if (this.searchControl.invalid) {
      return;
    }

    const term = raw?.trim() || null;
    this.searchTermUpdate.emit(term);
  }
}
