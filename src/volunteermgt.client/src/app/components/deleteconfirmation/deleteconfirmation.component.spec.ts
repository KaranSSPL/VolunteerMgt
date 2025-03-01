import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteconfirmationComponent } from './deleteconfirmation.component';

describe('DeleteconfirmationComponent', () => {
  let component: DeleteconfirmationComponent;
  let fixture: ComponentFixture<DeleteconfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DeleteconfirmationComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeleteconfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
