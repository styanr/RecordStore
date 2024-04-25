import { Box, Flex, Text } from '@chakra-ui/react';

const ContactInfo = ({ user }) => {
  return (
    <Box>
      <Flex my='3'>
        <Text as='b'>Ім'я:</Text>
        <Text ms='auto'>{user.firstName}</Text>
      </Flex>
      <Flex my='3'>
        <Text as='b'>Прізвище:</Text>
        <Text ms='auto'>{user.lastName}</Text>
      </Flex>
      <Flex my='3'>
        <Text as='b'>Електронна пошта:</Text>
        <Text ms='auto'>{user.email}</Text>
      </Flex>
    </Box>
  );
};

export default ContactInfo;
