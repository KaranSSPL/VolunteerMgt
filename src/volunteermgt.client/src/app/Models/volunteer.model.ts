export interface Volunteer {
  id: number;
  name: string;
  mobileNo: string;
  address: string;
  occupation: string;
  availabilities: { day: string; timeSlot: string }[];
}
