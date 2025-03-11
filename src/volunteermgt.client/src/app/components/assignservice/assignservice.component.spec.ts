import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignserviceComponent } from './assignservice.component';

describe('AssignserviceComponent', () => {
  let component: AssignserviceComponent;
  let fixture: ComponentFixture<AssignserviceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AssignserviceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AssignserviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
