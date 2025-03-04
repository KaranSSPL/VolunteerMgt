import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditvolunteerdialogComponent } from './editvolunteerdialog.component';

describe('EditvolunteerdialogComponent', () => {
  let component: EditvolunteerdialogComponent;
  let fixture: ComponentFixture<EditvolunteerdialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditvolunteerdialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditvolunteerdialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
