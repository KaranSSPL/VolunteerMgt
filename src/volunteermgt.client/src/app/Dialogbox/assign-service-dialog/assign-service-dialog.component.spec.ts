import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignServiceDialogComponent } from './assign-service-dialog.component';

describe('AssignServiceDialogComponent', () => {
  let component: AssignServiceDialogComponent;
  let fixture: ComponentFixture<AssignServiceDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AssignServiceDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignServiceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
