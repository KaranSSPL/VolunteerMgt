import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddcoupondialogComponent } from './addcoupondialog.component';

describe('AddcoupondialogComponent', () => {
  let component: AddcoupondialogComponent;
  let fixture: ComponentFixture<AddcoupondialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AddcoupondialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddcoupondialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
