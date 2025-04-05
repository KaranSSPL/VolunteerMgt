import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditVolunteerServiceComponent } from './edit-volunteer-service.component';

describe('EditVolunteerServiceComponent', () => {
  let component: EditVolunteerServiceComponent;
  let fixture: ComponentFixture<EditVolunteerServiceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditVolunteerServiceComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditVolunteerServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
