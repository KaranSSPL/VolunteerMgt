import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HandlekeydownService {
  selectedVolunteerIndex: number = -1;
  selectedServiceIndex: number = -1;
  constructor() { }

//  handleKeydown(event: KeyboardEvent, type: string) {
//    if (type === 'volunteer') {
//      if (this.searchSuggestions.length === 0) return;
//      if (event.key === 'ArrowDown') {
//        event.preventDefault();
//        this.selectedVolunteerIndex = (this.selectedVolunteerIndex + 1) % this.searchSuggestions.length;
//      } else if (event.key === 'ArrowUp') {
//        event.preventDefault();
//        this.selectedVolunteerIndex = (this.selectedVolunteerIndex - 1 + this.searchSuggestions.length) % this.searchSuggestions.length;
//      } else if (event.key === 'Enter' && this.selectedVolunteerIndex >= 0) {
//        event.preventDefault();
//        this.selectSuggestion(this.searchSuggestions[this.selectedVolunteerIndex]);
//      }
//    } else if (type === 'service') {
//      if (this.serviceSuggestions.length === 0) return;
//      if (event.key === 'ArrowDown') {
//        event.preventDefault();
//        this.selectedServiceIndex = (this.selectedServiceIndex + 1) % this.serviceSuggestions.length;
//      } else if (event.key === 'ArrowUp') {
//        event.preventDefault();
//        this.selectedServiceIndex = (this.selectedServiceIndex - 1 + this.serviceSuggestions.length) % this.serviceSuggestions.length;
//      } else if (event.key === 'Enter' && this.selectedServiceIndex >= 0) {
//        event.preventDefault();
//        this.selectService(this.serviceSuggestions[this.selectedServiceIndex]);
//      }
//    }
//  }
}
