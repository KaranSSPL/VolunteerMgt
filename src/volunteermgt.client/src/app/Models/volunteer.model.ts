export interface Volunteer {
  id: number;
  name: string;
  mobileNo: string;
  address: string;
  occupation: string;
  image: string;
  imagePath: string;
  code: string;
  volunteerType: string;
  availabilities: { day: string; timeSlot: string }[];
}
