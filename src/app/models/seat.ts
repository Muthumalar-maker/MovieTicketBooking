export interface Seat {
  seatId: number;
  screenId: number;
  seatNumber: string;
  rowNumber: string;
  seatType: string;
  isBooked: boolean;
  isSelected?: boolean;
  price: number;
}